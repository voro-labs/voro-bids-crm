"use client"

import { createContext, useContext, type ReactNode } from "react"

export interface SlugTenant {
  name: string
  slug: string
  logoUrl: string | null
  primaryColor: string | null
  secondaryColor: string | null
}

const SlugTenantContext = createContext<SlugTenant | null>(null)

export function SlugTenantProvider({
  children,
  tenant,
}: {
  children: ReactNode
  tenant: SlugTenant
}) {
  return (
    <SlugTenantContext.Provider value={tenant}>
      {children}
    </SlugTenantContext.Provider>
  )
}

export function useSlugTenant(): SlugTenant | null {
  return useContext(SlugTenantContext)
}
