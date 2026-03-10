"use client"

import { useState, useEffect, useRef, type ReactNode } from "react"
import { PiggyBank } from "lucide-react"
import type { SlugTenant } from "@/contexts/slug-tenant.context"

interface Props {
  tenant: SlugTenant | null
  icon?: ReactNode
}

export function TenantBrandHeader({ tenant, icon }: Props) {
  const [logoBlobUrl, setLogoBlobUrl] = useState<string | null>(null)
  const isMountedRef = useRef(true)

  useEffect(() => {
    isMountedRef.current = true
    return () => { isMountedRef.current = false }
  }, [])

  useEffect(() => {
    if (!tenant?.logoUrl) return

    let objectUrl: string | null = null

    const fetchLogo = async () => {
      try {
        const proxyUrl = `/api/blob/proxy?url=${encodeURIComponent(tenant.logoUrl!)}`
        const res = await fetch(proxyUrl)
        if (!res.ok) throw new Error("Failed to fetch logo")
        const blob = await res.blob()
        objectUrl = URL.createObjectURL(blob)
        if (isMountedRef.current) setLogoBlobUrl(objectUrl)
        else URL.revokeObjectURL(objectUrl)
      } catch {
        if (isMountedRef.current) setLogoBlobUrl(null)
      }
    }

    fetchLogo()

    return () => { if (objectUrl) URL.revokeObjectURL(objectUrl) }
  }, [tenant?.logoUrl])

  const logoSrc = logoBlobUrl ?? tenant?.logoUrl

  return (
    <div className="mb-8 flex flex-col items-center gap-3">
      {logoSrc ? (
        <img
          src={logoSrc}
          alt={tenant?.name ?? "Logo"}
          className="h-14 w-14 rounded-2xl object-contain shadow-md"
        />
      ) : (
        <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-primary text-primary-foreground shadow-md">
          {icon ?? <PiggyBank className="h-7 w-7" />}
        </div>
      )}
      <div className="text-center">
        <h1 className="text-2xl font-bold text-foreground">
          {tenant?.name ?? "Bids CRM"}
        </h1>
        <p className="mt-1 text-sm text-muted-foreground">Gerenciamento de clientes e serviços</p>
      </div>
    </div>
  )
}
