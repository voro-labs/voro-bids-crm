const LS_BRANDING_KEY = "voro:tenantBranding"
export const TENANT_SLUG_COOKIE = "voro_tenant_slug"

export interface TenantBranding {
  name: string | null
  logoUrl: string | null
}

export function saveTenantBranding(name: string | null, logoUrl: string | null, slug?: string | null) {
  if (typeof window === "undefined") return
  try {
    localStorage.setItem(LS_BRANDING_KEY, JSON.stringify({ name, logoUrl }))
  } catch { }
  // Also persist slug as a cookie so server-side generateMetadata can read it
  if (slug) {
    document.cookie = `${TENANT_SLUG_COOKIE}=${encodeURIComponent(slug)}; path=/; max-age=31536000; SameSite=Lax`
  }
}

export function loadTenantBranding(): TenantBranding {
  if (typeof window === "undefined") return { name: null, logoUrl: null }
  try {
    const raw = localStorage.getItem(LS_BRANDING_KEY)
    if (!raw) return { name: null, logoUrl: null }
    return JSON.parse(raw) as TenantBranding
  } catch {
    return { name: null, logoUrl: null }
  }
}

export function clearTenantBranding() {
  if (typeof window === "undefined") return
  try {
    localStorage.removeItem(LS_BRANDING_KEY)
  } catch { }
  // Clear the slug cookie
  document.cookie = `${TENANT_SLUG_COOKIE}=; path=/; max-age=0; SameSite=Lax`
}

export function applyTenantBranding(name: string | null, logoUrl: string | null) {
  if (name) {
    document.title = `${name} - CRM`
  }
  if (logoUrl) {
    let link = document.querySelector("link[rel~='icon']") as HTMLLinkElement | null
    if (!link) {
      link = document.createElement("link")
      link.rel = "icon"
      document.head.appendChild(link)
    }
    link.href = logoUrl
  }
}
