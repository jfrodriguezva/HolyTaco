export type MenuCategory =
  | "Tacos"
  | "Antojitos"
  | "Platillos"
  | "Cocteleria"
  | "Cervezas"
  | "Bebidas"
  | "Postres";

export interface MenuItemDto {
  id: string;
  name: string;
  description: string;
  price: number;
  currency: string;
  category: MenuCategory;
  isSpicy: boolean;
  isAvailable: boolean;
  imageUrl: string | null;
}

export interface OrderItemDto {
  id: string;
  menuItemId: string;
  menuItemName: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
  notes: string | null;
}

export interface OrderDto {
  id: string;
  tableNumber: number;
  customerName: string | null;
  status: string;
  createdAtUtc: string;
  total: number;
  items: OrderItemDto[];
}

export interface CreateOrderItemRequest {
  menuItemId: string;
  quantity: number;
  notes?: string | null;
}

export interface CreateOrderRequest {
  tableNumber: number;
  customerName?: string | null;
  items: CreateOrderItemRequest[];
}

export type DiscountType = "Percentage" | "FixedAmount" | "ComboPrice";

export interface PromotionDto {
  id: string;
  title: string;
  description: string;
  imageUrl: string | null;
  discountType: DiscountType;
  discountValue: number | null;
  comboPrice: number | null;
  menuItemIds: string[];
  startsAtUtc: string;
  endsAtUtc: string;
  isFeatured: boolean;
  isActive: boolean;
  isCurrentlyValid: boolean;
}

export interface CreatePromotionRequest {
  title: string;
  description: string;
  imageUrl?: string | null;
  discountType: DiscountType;
  discountValue?: number | null;
  comboPrice?: number | null;
  menuItemIds: string[];
  startsAtUtc: string;
  endsAtUtc: string;
  isFeatured: boolean;
}

export type ReservationStatus = "Pendiente" | "Confirmada" | "Cancelada" | "Completada";

export interface ReservationDto {
  id: string;
  customerName: string;
  phone: string;
  email: string | null;
  partySize: number;
  reservationAtUtc: string;
  tableNumber: number;
  status: ReservationStatus;
  notes: string | null;
  createdAtUtc: string;
}

export interface CreateReservationRequest {
  customerName: string;
  phone: string;
  email?: string | null;
  partySize: number;
  reservationAtUtc: string;
  notes?: string | null;
}

export type StaffRole = "Mesero" | "Cocina" | "Gerente";

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  role: StaffRole;
  expiresAtUtc: string;
}

function authHeader(token?: string): Record<string, string> {
  return token ? { Authorization: `Bearer ${token}` } : {};
}

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5100";

async function apiFetch<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...init?.headers,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    const body = await response.text().catch(() => "");
    let message = body || `Error ${response.status} al llamar ${path}`;
    try {
      const parsed = JSON.parse(body) as { message?: string; title?: string };
      message = parsed.message ?? parsed.title ?? message;
    } catch {
      // el cuerpo no era JSON; se usa el texto plano como mensaje
    }
    throw new Error(message);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export const authApi = {
  login: (request: LoginRequest) =>
    apiFetch<LoginResponse>("/api/auth/login", {
      method: "POST",
      body: JSON.stringify(request),
    }),
};

export const menuApi = {
  getAll: () => apiFetch<MenuItemDto[]>("/api/menu"),
  getByCategory: (category: MenuCategory) =>
    apiFetch<MenuItemDto[]>(`/api/menu?category=${encodeURIComponent(category)}`),
};

export const ordersApi = {
  create: (request: CreateOrderRequest) =>
    apiFetch<string>("/api/orders", {
      method: "POST",
      body: JSON.stringify(request),
    }),
  getById: (id: string) => apiFetch<OrderDto>(`/api/orders/${id}`),
  getActive: () => apiFetch<OrderDto[]>("/api/orders"),
};

export const promotionsApi = {
  getActive: () => apiFetch<PromotionDto[]>("/api/promotions?onlyActive=true"),
  getFeatured: () => apiFetch<PromotionDto[]>("/api/promotions/featured"),
  getAll: () => apiFetch<PromotionDto[]>("/api/promotions"),
  create: (request: CreatePromotionRequest, token: string) =>
    apiFetch<string>("/api/promotions", {
      method: "POST",
      headers: authHeader(token),
      body: JSON.stringify(request),
    }),
  deactivate: (id: string, token: string) =>
    apiFetch<void>(`/api/promotions/${id}/deactivate`, { method: "POST", headers: authHeader(token) }),
};

export const reservationsApi = {
  getAvailability: (date: string, partySize: number) =>
    apiFetch<string[]>(`/api/reservations/availability?date=${date}&partySize=${partySize}`),
  create: (request: CreateReservationRequest) =>
    apiFetch<string>("/api/reservations", {
      method: "POST",
      body: JSON.stringify(request),
    }),
  getByDate: (date: string) => apiFetch<ReservationDto[]>(`/api/reservations?date=${date}`),
  getById: (id: string) => apiFetch<ReservationDto>(`/api/reservations/${id}`),
  confirm: (id: string, token: string) =>
    apiFetch<void>(`/api/reservations/${id}/confirm`, { method: "POST", headers: authHeader(token) }),
  cancel: (id: string, token: string) =>
    apiFetch<void>(`/api/reservations/${id}/cancel`, { method: "POST", headers: authHeader(token) }),
};
