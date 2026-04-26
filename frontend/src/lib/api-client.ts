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

function buildUrl(path: string, params?: object): string {
  const url = `${BASE_URL}${path}`;

  if (!params) return url;

  const qs = new URLSearchParams();
  for (const [key, value] of Object.entries(params)) {
    if (value !== undefined && value !== null) {
      qs.set(key, String(value));
    }
  }

  const queryString = qs.toString();
  return queryString ? `${url}?${queryString}` : url;
}

async function request<T>(
  method: string,
  path: string,
  body?: unknown,
  params?: object,
): Promise<T> {
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    Accept: "application/json",
  };

  const init: RequestInit = { method, headers };
  if (body !== undefined) {
    init.body = JSON.stringify(body);
  }

  const res = await fetch(buildUrl(path, params), init);

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
  get: <T>(path: string, params?: object) => request<T>("GET", path, undefined, params),
  post: <T>(path: string, body: unknown) => request<T>("POST", path, body),
  put: <T>(path: string, body: unknown) => request<T>("PUT", path, body),
  delete: (path: string): Promise<void> => request("DELETE", path),
} as const;
