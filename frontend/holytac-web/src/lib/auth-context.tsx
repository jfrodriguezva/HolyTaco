"use client";

import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import { authApi, type StaffRole } from "./api";

export interface StaffSession {
  token: string;
  username: string;
  role: StaffRole;
  expiresAtUtc: string;
}

interface AuthContextValue {
  session: StaffSession | null;
  hydrated: boolean;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);
const STORAGE_KEY = "holytac.staffSession";

function readStoredSession(): StaffSession | null {
  try {
    const raw = window.localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    const session = JSON.parse(raw) as StaffSession;
    if (new Date(session.expiresAtUtc) <= new Date()) return null;
    return session;
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<StaffSession | null>(null);
  const [hydrated, setHydrated] = useState(false);

  useEffect(() => {
    setSession(readStoredSession());
    setHydrated(true);
  }, []);

  const login = async (username: string, password: string) => {
    const response = await authApi.login({ username, password });
    const newSession: StaffSession = {
      token: response.token,
      username: response.username,
      role: response.role,
      expiresAtUtc: response.expiresAtUtc,
    };
    try {
      window.localStorage.setItem(STORAGE_KEY, JSON.stringify(newSession));
    } catch {
      // localStorage no disponible; la sesión solo vive en memoria para esta pestaña
    }
    setSession(newSession);
  };

  const logout = () => {
    try {
      window.localStorage.removeItem(STORAGE_KEY);
    } catch {
      // localStorage no disponible
    }
    setSession(null);
  };

  return <AuthContext.Provider value={{ session, hydrated, login, logout }}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth debe usarse dentro de un AuthProvider");
  return ctx;
}
