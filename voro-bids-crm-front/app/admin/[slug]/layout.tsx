import { notFound } from "next/navigation"
import type { ReactNode } from "react"
import { API_CONFIG } from "@/lib/api"
import { SlugTenantProvider, type SlugTenant } from "@/contexts/slug-tenant.context"
import { SlugThemeApplier } from "./theme-applier"

interface Props {
  children: ReactNode
  params: Promise<{ slug: string }>
}

async function fetchTenantBySlug(slug: string): Promise<SlugTenant | null> {
  try {
    const url = `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.TENANT_BY_SLUG}/${slug}`
    const res = await fetch(url, {
      cache: "no-store",
      headers: { Accept: "application/json" },
    })
    if (!res.ok) return null
    const json = await res.json()
    if (json?.hasError || !json?.data) return null
    return json.data as SlugTenant
  } catch {
    return null
  }
}

export default async function SlugAdminLayout({ children, params }: Props) {
  const { slug } = await params
  const tenant = await fetchTenantBySlug(slug)

  if (!tenant) notFound()

  return (
    <SlugTenantProvider tenant={tenant}>
      <SlugThemeApplier
        primaryColor={tenant.primaryColor}
        secondaryColor={tenant.secondaryColor}
      />
      {children}
    </SlugTenantProvider>
  )
}
