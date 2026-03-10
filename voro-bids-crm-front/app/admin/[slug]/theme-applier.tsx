"use client"

import { useEffect } from "react"

function hexToOklch(hex: string): { str: string; l: number } | null {
  const m = hex.match(/^#?([0-9a-f]{6})$/i)
  if (!m) return null
  const n = parseInt(m[1], 16)
  let r = ((n >> 16) & 0xff) / 255
  let g = ((n >> 8) & 0xff) / 255
  let b = (n & 0xff) / 255

  const lin = (v: number) =>
    v <= 0.04045 ? v / 12.92 : Math.pow((v + 0.055) / 1.055, 2.4)
  r = lin(r); g = lin(g); b = lin(b)

  const X = 0.4124 * r + 0.3576 * g + 0.1805 * b
  const Y = 0.2126 * r + 0.7152 * g + 0.0722 * b
  const Z = 0.0193 * r + 0.1192 * g + 0.9505 * b

  const l_ = Math.cbrt(0.8189330101 * X + 0.3618667424 * Y - 0.1288597137 * Z)
  const m_ = Math.cbrt(0.0329845436 * X + 0.9293118715 * Y + 0.0361456387 * Z)
  const s_ = Math.cbrt(0.0482003018 * X + 0.2643662691 * Y + 0.6338517070 * Z)

  const L = 0.2104542553 * l_ + 0.7936177850 * m_ - 0.0040720468 * s_
  const a = 1.9779984951 * l_ - 2.4285922050 * m_ + 0.4505937099 * s_
  const bOk = 0.0259040371 * l_ + 0.7827717662 * m_ - 0.8086757660 * s_

  const C = Math.sqrt(a * a + bOk * bOk)
  const H = (Math.atan2(bOk, a) * 180) / Math.PI
  const deg = H < 0 ? H + 360 : H

  return {
    str: `oklch(${L.toFixed(4)} ${C.toFixed(4)} ${deg.toFixed(2)})`,
    l: L,
  }
}

interface Props {
  primaryColor: string | null
  secondaryColor: string | null
}

export function SlugThemeApplier({ primaryColor, secondaryColor }: Props) {
  useEffect(() => {
    const root = document.documentElement

    if (primaryColor) {
      const ok = hexToOklch(primaryColor)
      if (ok) {
        root.style.setProperty("--primary", ok.str)
        root.style.setProperty("--ring", ok.str)
        root.style.setProperty("--sidebar-primary", ok.str)
        const fg = ok.l > 0.6 ? "oklch(0.10 0.01 285)" : "oklch(0.985 0.002 75)"
        root.style.setProperty("--primary-foreground", fg)
        root.style.setProperty("--sidebar-primary-foreground", fg)
      }
    }

    if (secondaryColor) {
      const ok = hexToOklch(secondaryColor)
      if (ok) {
        root.style.setProperty("--accent", ok.str)
        root.style.setProperty("--sidebar-accent", ok.str)
        const fg = ok.l > 0.6 ? "oklch(0.10 0.01 285)" : "oklch(0.985 0.002 75)"
        root.style.setProperty("--accent-foreground", fg)
        root.style.setProperty("--sidebar-accent-foreground", fg)
      }
    }
  }, [primaryColor, secondaryColor])

  return null
}
