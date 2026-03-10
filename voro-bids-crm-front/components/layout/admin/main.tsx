"use client"

import type React from "react"
import { useState, useEffect, useRef } from "react"
import { usePathname, useRouter } from "next/navigation"
import { LoadingSimple } from "../../ui/custom/loading/loading-simple"
import { useAuth } from "@/contexts/auth.context"
import { routesAllowed, routesAllowedPatterns } from "@/lib/allowed-utils"
import { Navbar } from "./navbar"
import { Sidebar } from "./sidebar"
import useSWR from "swr"
import { API_CONFIG, secureApiCall } from "@/lib/api"

import { ThemeToggle } from "../../theme-toggle"

interface MainProps {
  children: React.ReactNode
}

export function Main({ children }: MainProps) {
  const { user, loading } = useAuth()
  const [isSidebarOpen, setIsSidebarOpen] = useState(false)
  const [tenantLogoBlobUrl, setTenantLogoBlobUrl] = useState<string | null>(null)
  const isMountedRef = useRef(true)
  const router = useRouter()
  const pathname = usePathname()

  const { data: tenant } = useSWR(
    user?.token ? API_CONFIG.ENDPOINTS.TENANT_ME : null,
    async (url) => {
      const res = await secureApiCall<any>(url, { method: "GET" })
      if (res.hasError) throw new Error(res.message || "Failed to fetch tenant")
      return res.data
    }
  )

  useEffect(() => {
    isMountedRef.current = true
    return () => {
      isMountedRef.current = false
    }
  }, [])

  useEffect(() => {
    if (!tenant?.logoUrl) return

    let objectUrl: string | null = null

    const fetchSignedUrl = async () => {
      try {
        const proxyUrl = `/api/blob/proxy?url=${encodeURIComponent(tenant.logoUrl)}`
        const response = await fetch(proxyUrl)

        if (!response.ok) throw new Error("Failed to fetch signed URL via proxy")

        const blob = await response.blob()
        objectUrl = URL.createObjectURL(blob)

        if (isMountedRef.current) {
          setTenantLogoBlobUrl(objectUrl)
        } else {
          URL.revokeObjectURL(objectUrl)
        }
      } catch (err) {
        console.error("Error fetching tenant logo:", err)
        if (isMountedRef.current) setTenantLogoBlobUrl(null)
      }
    }

    fetchSignedUrl()

    return () => {
      if (objectUrl) URL.revokeObjectURL(objectUrl)
    }
  }, [tenant?.logoUrl])

  useEffect(() => {
    if (tenant?.name) {
      document.title = `${tenant.name} - CRM`
    }
    const logoHref = tenantLogoBlobUrl ?? tenant?.logoUrl
    if (logoHref) {
      let link = document.querySelector("link[rel~='icon']") as HTMLLinkElement
      if (!link) {
        link = document.createElement("link")
        link.rel = "icon"
        document.head.appendChild(link)
      }
      link.href = logoHref
    }
  }, [tenant, tenantLogoBlobUrl])

  if (loading) {
    return <LoadingSimple />
  }

  if (!user?.token) {
    const isPublicRoute = routesAllowed.some(item => pathname.startsWith(item))
      || routesAllowedPatterns.some(pattern => pattern.test(pathname))

    if (!isPublicRoute) {
      // Rota protegida sem usuário: redireciona silenciosamente
      router.replace("/admin/sign-in")
    }

    // Layout público (sign-in, forgot-password, etc.) — nunca mostra loading aqui
    return (
      <div className="min-h-screen bg-background text-foreground">
        <div className="fixed top-4 right-4 z-50">
          <ThemeToggle />
        </div>
        <main className="flex-1">{children}</main>
      </div>
    )
  }

  // Layout autenticado
  return (
    <div
      className="min-h-screen"
      style={{
        ...(tenant?.primaryColor ? { "--primary": tenant.primaryColor } : {}),
        ...(tenant?.secondaryColor ? { "--secondary": tenant.secondaryColor } : {}),
      } as React.CSSProperties}
    >
      <div className="flex">
        <Sidebar isOpen={isSidebarOpen} onClose={() => setIsSidebarOpen(false)} tenant={tenant} />

        <div className="flex-1">
          <Navbar isOpen={isSidebarOpen} onMenuClick={() => setIsSidebarOpen(true)} tenant={tenant} />
          <main>{children}</main>
        </div>
      </div>
    </div>
  )
}
