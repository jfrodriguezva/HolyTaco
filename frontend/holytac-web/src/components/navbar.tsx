"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useCart } from "@/lib/cart-context";

const links = [
  { href: "/", label: "Inicio" },
  { href: "/menu", label: "Menú" },
  { href: "/promociones", label: "Promociones" },
  { href: "/reservar", label: "Reservar" },
];

export function Navbar() {
  const pathname = usePathname();
  const { itemCount } = useCart();

  return (
    <header className="sticky top-0 z-40 border-b border-agave/15 bg-cream/90 backdrop-blur">
      <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-4">
        <Link href="/" className="font-display text-2xl font-semibold tracking-tight text-agave">
          Holy<span className="text-terracota">Tac</span>
        </Link>

        <nav className="hidden items-center gap-8 sm:flex">
          {links.map((link) => (
            <Link
              key={link.href}
              href={link.href}
              className={`text-sm font-medium uppercase tracking-wide transition-colors ${
                pathname === link.href ? "text-terracota" : "text-ink-soft hover:text-terracota"
              }`}
            >
              {link.label}
            </Link>
          ))}
        </nav>

        <div className="flex items-center gap-3">
          <Link
            href="/reservar"
            className="hidden rounded-full bg-terracota px-4 py-2 text-sm font-semibold text-cream transition-colors hover:bg-terracota-dark sm:inline-block"
          >
            Reservar mesa
          </Link>
          <Link
            href="/carrito"
            aria-label="Carrito"
            className="relative flex h-9 w-9 items-center justify-center rounded-full border border-agave/20 text-ink-soft transition-colors hover:border-terracota hover:text-terracota"
          >
            🛒
            {itemCount > 0 && (
              <span className="absolute -right-1 -top-1 flex h-4 min-w-4 items-center justify-center rounded-full bg-chile px-1 text-[10px] font-bold text-ink">
                {itemCount}
              </span>
            )}
          </Link>
        </div>
      </div>
    </header>
  );
}
