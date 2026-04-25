export class ApiError extends Error {
  readonly status: number;
  readonly detail?: string;

  constructor(status: number, message: string, detail?: string) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    if (detail !== undefined) {
      this.detail = detail;
    }
  }
}

const BASE_URL = import.meta.env.VITE_API_BASE_URL;

async function request<T>(method: string, path: string, body?: unknown): Promise<T> {
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    Accept: "application/json",
  };

  const init: RequestInit = { method, headers };
  if (body !== undefined) {
    init.body = JSON.stringify(body);
  }

  const res = await fetch(`${BASE_URL}${path}`, init);

  if (!res.ok) {
    let message = res.statusText;
    let detail: string | undefined;
    try {
      const json = (await res.json()) as { message?: string; detail?: string };
      if (json.message) message = json.message;
      if (json.detail) detail = json.detail;
    } catch {
      /* non-JSON body */
    }
    throw new ApiError(res.status, message, detail);
  }

  if (res.status === 204) return undefined as T;

  return res.json() as Promise<T>;
}

export const api = {
  get: <T>(path: string) => request<T>("GET", path),
  post: <T>(path: string, body: unknown) => request<T>("POST", path, body),
  put: <T>(path: string, body: unknown) => request<T>("PUT", path, body),
  delete: (path: string): Promise<void> => request("DELETE", path),
} as const;
