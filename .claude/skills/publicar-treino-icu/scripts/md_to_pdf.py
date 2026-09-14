#!/usr/bin/env python3
"""Converte um markdown simples (headers #/##/###, tabelas |a|b|, listas -, bold **x**,
blockquote >, code fences ```) para PDF usando fpdf2 (sem dependências de sistema).

Uso: python3 md_to_pdf.py <entrada.md> <saida.pdf>
"""
import re
import sys

from fpdf import FPDF

PAGE_WIDTH = 210
MARGIN = 15
CONTENT_WIDTH = PAGE_WIDTH - 2 * MARGIN


REPLACEMENTS = {
    "→": "->",
    "–": "-",
    "—": "-",
    "‘": "'",
    "’": "'",
    "“": '"',
    "”": '"',
    "…": "...",
    "×": "x",
}


def sanitize(text):
    for src, dst in REPLACEMENTS.items():
        text = text.replace(src, dst)
    return text.encode("latin-1", errors="replace").decode("latin-1")


def parse_inline_bold(pdf, text, line_height=6):
    text = sanitize(text)
    parts = re.split(r"(\*\*.+?\*\*)", text)
    for part in parts:
        if part.startswith("**") and part.endswith("**"):
            pdf.set_font(style="B")
            pdf.write(line_height, part[2:-2])
            pdf.set_font(style="")
        else:
            pdf.write(line_height, part)


class TrainingPdf(FPDF):
    def header(self):
        pass

    def footer(self):
        self.set_y(-12)
        self.set_font("Helvetica", size=8)
        self.set_text_color(140, 140, 140)
        self.cell(0, 8, f"{self.page_no()}", align="C")


def _row_height(pdf, cells, col_width, line_height):
    max_lines = 1
    for cell in cells:
        lines = pdf.multi_cell(col_width, line_height, cell, dry_run=True, output="LINES")
        max_lines = max(max_lines, len(lines) or 1)
    return max_lines * line_height


def _draw_row(pdf, cells, col_width, row_h, line_height, bold=False, fill=False):
    x0, y0 = pdf.get_x(), pdf.get_y()
    if bold:
        pdf.set_font(style="B")
    if fill:
        pdf.set_fill_color(230, 230, 230)
    x = x0
    for cell in cells:
        pdf.set_xy(x, y0)
        pdf.multi_cell(col_width, line_height, cell, border=1, align="L", fill=fill)
        x += col_width
    if bold:
        pdf.set_font(style="")
    pdf.set_xy(x0, y0 + row_h)


def render_table(pdf, rows):
    if not rows:
        return
    rows = [[sanitize(c) for c in row] for row in rows]
    n_cols = len(rows[0])
    col_width = CONTENT_WIDTH / n_cols
    line_height = 5
    header = rows[0]
    body = rows[2:] if len(rows) > 1 and all(set(c) <= set("-: ") for c in rows[1]) else rows[1:]

    pdf.set_font("Helvetica", size=8)
    pdf.set_auto_page_break(False)

    pdf.set_font(style="B")
    header_h = _row_height(pdf, header, col_width, line_height)
    pdf.set_font(style="")
    first_row_padded = (body[0] + [""] * (n_cols - len(body[0]))) if body else []
    first_row_h = _row_height(pdf, first_row_padded, col_width, line_height) if body else 0
    if pdf.get_y() + header_h + first_row_h > pdf.page_break_trigger:
        pdf.add_page()
    _draw_row(pdf, header, col_width, header_h, line_height, bold=True, fill=True)

    for row in body:
        row = row + [""] * (n_cols - len(row))
        row_h = _row_height(pdf, row, col_width, line_height)
        if pdf.get_y() + row_h > pdf.page_break_trigger:
            pdf.add_page()
            _draw_row(pdf, header, col_width, header_h, line_height, bold=True, fill=True)
        _draw_row(pdf, row, col_width, row_h, line_height)

    pdf.set_auto_page_break(True, margin=18)


def convert(md_path, pdf_path):
    with open(md_path, encoding="utf-8") as f:
        lines = f.read().splitlines()

    pdf = TrainingPdf(format="A4")
    pdf.set_auto_page_break(auto=True, margin=18)
    pdf.set_margins(MARGIN, MARGIN, MARGIN)
    pdf.add_page()
    pdf.set_font("Helvetica", size=10)

    i = 0
    in_code = False
    code_buf = []
    table_buf = []

    def flush_table():
        nonlocal table_buf
        if table_buf:
            render_table(pdf, table_buf)
            pdf.ln(2)
            table_buf = []

    while i < len(lines):
        raw = lines[i]
        line = raw.rstrip()

        if line.strip().startswith("```"):
            if in_code:
                pdf.set_font("Courier", size=8)
                pdf.set_fill_color(245, 245, 245)
                pdf.multi_cell(CONTENT_WIDTH, 4.5, sanitize("\n".join(code_buf)), border=1, fill=True)
                pdf.ln(2)
                pdf.set_font("Helvetica", size=10)
                code_buf = []
                in_code = False
            else:
                flush_table()
                in_code = True
            i += 1
            continue

        if in_code:
            code_buf.append(raw)
            i += 1
            continue

        if line.strip().startswith("|"):
            cells = [c.strip() for c in line.strip().strip("|").split("|")]
            table_buf.append(cells)
            i += 1
            continue
        else:
            flush_table()

        if not line.strip():
            pdf.ln(3)
            i += 1
            continue

        m = re.match(r"^(#{1,4})\s+(.*)", line)
        if m:
            level = len(m.group(1))
            text = sanitize(m.group(2))
            sizes = {1: 18, 2: 14, 3: 12, 4: 11}
            pdf.set_font("Helvetica", style="B", size=sizes.get(level, 10))
            pdf.ln(2)
            pdf.multi_cell(CONTENT_WIDTH, 8, text)
            pdf.set_font("Helvetica", size=10)
            pdf.ln(1)
            i += 1
            continue

        if line.strip().startswith(">"):
            text = sanitize(line.strip().lstrip(">").strip())
            pdf.set_font("Helvetica", style="I", size=9)
            pdf.set_text_color(90, 90, 90)
            pdf.multi_cell(CONTENT_WIDTH, 5.5, text)
            pdf.set_text_color(0, 0, 0)
            pdf.set_font("Helvetica", size=10)
            i += 1
            continue

        if re.match(r"^[-*]\s+", line.strip()):
            text = re.sub(r"^[-*]\s+", "", line.strip())
            pdf.set_x(MARGIN + 4)
            pdf.write(5.5, "- ")
            parse_inline_bold(pdf, text, 5.5)
            pdf.ln(5.5)
            i += 1
            continue

        parse_inline_bold(pdf, line, 5.5)
        pdf.ln(5.5)
        i += 1

    flush_table()
    pdf.output(pdf_path)


if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Uso: md_to_pdf.py <entrada.md> <saida.pdf>", file=sys.stderr)
        sys.exit(1)
    convert(sys.argv[1], sys.argv[2])
