"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth-context";

export default function LoginPage() {
  const { login, session } = useAuth();
  const router = useRouter();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      await login(username, password);
      router.push("/admin/reservaciones");
    } catch (err) {
      setError(err instanceof Error ? err.message : "No pudimos iniciar sesión.");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="mx-auto max-w-sm px-6 py-24">
      <header className="mb-8 text-center">
        <span className="text-xs font-semibold uppercase tracking-[0.2em] text-terracota">Staff</span>
        <h1 className="font-display mt-1 text-3xl font-semibold text-agave">Iniciar sesión</h1>
      </header>

      {session && (
        <p className="mb-6 rounded-lg border border-agave/20 bg-agave/10 p-3 text-center text-sm text-agave">
          Ya iniciaste sesión como <strong>{session.username}</strong> ({session.role}).
        </p>
      )}

      <form onSubmit={handleSubmit} className="space-y-4 rounded-2xl border border-agave/10 bg-white/50 p-6">
        <label className="flex flex-col gap-1 text-sm text-ink-soft">
          Usuario
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            autoComplete="username"
            className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
          />
        </label>
        <label className="flex flex-col gap-1 text-sm text-ink-soft">
          Contraseña
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            autoComplete="current-password"
            className="rounded-lg border border-agave/20 bg-cream px-3 py-2 text-ink outline-none focus:border-terracota"
          />
        </label>

        {error && <p className="text-sm font-medium text-terracota-dark">{error}</p>}

        <button
          type="submit"
          disabled={submitting}
          className="w-full rounded-full bg-terracota py-3 text-sm font-semibold uppercase tracking-wide text-cream transition-colors hover:bg-terracota-dark disabled:opacity-60"
        >
          {submitting ? "Entrando…" : "Entrar"}
        </button>
      </form>
    </div>
  );
}
