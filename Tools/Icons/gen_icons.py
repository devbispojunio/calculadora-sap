# Gera o ícone próprio da Calculadora SAP sobrescrevendo os assets Calculator*.png
# (mesmos nomes e tamanhos dos arquivos originais).
# Uso: python3 gen_icons.py <pasta Assets> <fonte .ttf bold>
import os
import struct
import sys

from PIL import Image, ImageDraw, ImageFont

ASSETS = sys.argv[1]
FONT = sys.argv[2]
BLUE_TOP = (24, 136, 255)   # gradiente do azul do tema (#0070F2)
BLUE_BOTTOM = (0, 92, 212)
SS = 4  # supersampling


def png_size(path):
    with open(path, "rb") as f:
        d = f.read(24)
    return struct.unpack(">II", d[16:24])


def draw_glyph(img, box, color, detailed):
    """Desenha 'UM' + seta de conversão dentro de box=(x0,y0,x1,y1)."""
    d = ImageDraw.Draw(img)
    x0, y0, x1, y1 = box
    bw, bh = x1 - x0, y1 - y0
    text = "UM"
    # tamanho da fonte para ocupar ~70% da largura
    size = int(bh * 0.6)
    while size > 4:
        font = ImageFont.truetype(FONT, size)
        l, t, r, b = d.textbbox((0, 0), text, font=font)
        if (r - l) <= bw * 0.74:
            break
        size -= max(1, size // 40)
    tw, th = r - l, b - t
    arrow_h = bh * 0.16 if detailed else 0
    gap = bh * 0.07 if detailed else 0
    total = th + gap + arrow_h
    tx = x0 + (bw - tw) / 2 - l
    ty = y0 + (bh - total) / 2 - t
    d.text((tx, ty), text, font=font, fill=color)
    if detailed:
        # seta dupla (⇄ simplificada): linha com pontas nas duas extremidades
        ay = y0 + (bh - total) / 2 + th + gap + arrow_h / 2
        ax0 = x0 + (bw - tw) / 2
        ax1 = ax0 + tw
        lw = max(2, int(arrow_h * 0.28))
        head = arrow_h * 0.5
        d.line([(ax0 + head * 0.6, ay), (ax1 - head * 0.6, ay)], fill=color, width=lw)
        d.polygon([(ax0, ay), (ax0 + head, ay - head * 0.75), (ax0 + head, ay + head * 0.75)], fill=color)
        d.polygon([(ax1, ay), (ax1 - head, ay - head * 0.75), (ax1 - head, ay + head * 0.75)], fill=color)


def plate(size):
    """Quadrado azul arredondado com gradiente, tamanho size (já supersampled)."""
    grad = Image.new("RGBA", (size, size))
    gd = ImageDraw.Draw(grad)
    for y in range(size):
        k = y / max(1, size - 1)
        c = tuple(int(BLUE_TOP[i] + (BLUE_BOTTOM[i] - BLUE_TOP[i]) * k) for i in range(3)) + (255,)
        gd.line([(0, y), (size, y)], fill=c)
    mask = Image.new("L", (size, size), 0)
    ImageDraw.Draw(mask).rounded_rectangle([0, 0, size - 1, size - 1], radius=int(size * 0.22), fill=255)
    out = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    out.paste(grad, (0, 0), mask)
    return out


def plated_icon(w, h, frac):
    """Ícone com placa azul centralizado; frac = lado da placa / menor dimensão."""
    W, H = w * SS, h * SS
    img = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    side = int(min(W, H) * frac)
    p = plate(side)
    pad = int(side * 0.16)
    draw_glyph(p, (pad, pad, side - pad, side - pad), (255, 255, 255, 255), min(w, h) * frac >= 40)
    img.paste(p, ((W - side) // 2, (H - side) // 2), p)
    return img.resize((w, h), Image.LANCZOS)


def glyph_only(w, h, frac, color):
    W, H = w * SS, h * SS
    img = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    side = int(min(W, H) * frac)
    x0, y0 = (W - side) // 2, (H - side) // 2
    draw_glyph(img, (x0, y0, x0 + side, y0 + side), color, min(w, h) * frac >= 40)
    return img.resize((w, h), Image.LANCZOS)


count = 0
for name in sorted(os.listdir(ASSETS)):
    if not (name.startswith("Calculator") and name.endswith(".png")):
        continue
    path = os.path.join(ASSETS, name)
    w, h = png_size(path)
    kind = name.split(".")[0]  # CalculatorAppList, CalculatorMedTile...
    is_tile_like = kind in ("CalculatorMedTile", "CalculatorLargeTile", "CalculatorWideTile",
                            "CalculatorSmallTile", "CalculatorSplashScreen")
    frac = {"CalculatorAppList": 1.0, "CalculatorStoreLogo": 1.0}.get(kind, 0.5 if kind != "CalculatorSmallTile" else 0.62)

    if "contrast-black" in name:
        img = glyph_only(w, h, frac * (0.9 if not is_tile_like else 1), (255, 255, 255, 255))
    elif "contrast-white" in name:
        img = glyph_only(w, h, frac * (0.9 if not is_tile_like else 1), (0, 0, 0, 255))
    else:
        img = plated_icon(w, h, frac)

    # PNG de 8 bits com alfa (remove a ligação hardlink criando arquivo novo)
    tmp = path + ".tmp"
    img.save(tmp, "PNG", optimize=True)
    os.replace(tmp, path)
    count += 1

print("gerados:", count)
