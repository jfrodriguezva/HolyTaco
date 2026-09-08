"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/auth-context";
import type { StaffRole } from "@/lib/api";

/** Protege una página de `/admin/**`: exige sesión de staff y, opcionalmente, un rol específico. */
export function RequireStaffAuth({
  requiredRole,
  children,
}: {
  requiredRole?: StaffRole;
  children: React.ReactNode;
}) {
  const { session, hydrated } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (hydrated && !session) {
      router.replace("/login");
    }
  }, [hydrated, session, router]);

  if (!hydrated || !session) {
    return <div className="mx-auto max-w-5xl px-6 py-14 text-ink-soft">Verificando sesión…</div>;
  }

  if (requiredRole && session.role !== requiredRole) {
    return (
      <div className="mx-auto max-w-5xl px-6 py-14">
        <p className="rounded-2xl border border-terracota/30 bg-terracota/10 p-6 text-terracota-dark">
          Tu cuenta ({session.username}, rol {session.role}) no tiene permiso para ver esta sección. Se requiere
          el rol {requiredRole}.
        </p>
      </div>
    );
  }

  return <>{children}</>;
}
