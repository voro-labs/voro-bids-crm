"use client"

import { useState } from "react"
import useSWR from "swr"
import Link from "next/link"
import { MetricCard } from "@/components/metric-card"
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"
import { Button } from "@/components/ui/button"
import {
  Gavel,
  FileText,
  Clock,
  CheckCircle2,
  Circle,
  XCircle,
  Plus,
  ChevronRight,
  Loader2
} from "lucide-react"

import { API_CONFIG, secureApiCall } from "@/lib/api"
import { AuthGuard } from "@/components/auth/auth.guard"

const fetcher = async (url: string) => {
  const result = await secureApiCall<any>(url, { method: "GET" })
  if (result.hasError) throw new Error(result.message || "Error")
  return result.data
}

// Bidding status config (0: Setup, 1: Published, 2: Suspended, 3: Canceled, 4: Finished, 5: Won, 6: Lost)
const statusConfig: Record<number, { label: string; color: string; icon: any }> = {
  0: { label: "Em Estruturação", color: "bg-gray-100 text-gray-800 border-gray-200", icon: Circle },
  1: { label: "Publicado", color: "bg-blue-100 text-blue-800 border-blue-200", icon: Clock },
  2: { label: "Suspenso", color: "bg-yellow-100 text-yellow-800 border-yellow-200", icon: Circle },
  3: { label: "Cancelado", color: "bg-red-100 text-red-800 border-red-200", icon: XCircle },
  4: { label: "Encerrado", color: "bg-gray-200 text-gray-600 border-gray-300", icon: CheckCircle2 },
  5: { label: "Ganha", color: "bg-green-100 text-green-800 border-green-200", icon: CheckCircle2 },
  6: { label: "Perdida", color: "bg-red-100 text-red-800 border-red-200", icon: XCircle },
}

export default function DashboardPage() {
  const { data: auctions, isLoading } = useSWR(API_CONFIG.ENDPOINTS.AUCTIONS, fetcher)

  if (isLoading) {
    return (
      <div className="flex flex-col gap-6 p-6">
        <div>
          <h1 className="text-2xl font-bold text-foreground text-balance">Painel de Licitações</h1>
          <p className="text-sm text-muted-foreground mt-1">Visão geral das licitações e concorrências</p>
        </div>
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
          {[1, 2, 3].map((i) => (
            <Card key={i} className="border-border/60">
              <CardContent className="pt-6">
                <Skeleton className="h-4 w-24 mb-2 bg-muted" />
                <Skeleton className="h-8 w-32 bg-muted" />
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    )
  }

  const activeAuctions = auctions?.filter((a: any) => [0, 1].includes(a.status)) || [];
  const wonAuctions = auctions?.filter((a: any) => a.status === 5).length || 0;

  return (
    <AuthGuard requiredRoles={["User", "Admin", "Legal", "Finance", "Management", "Operational"]}>
      <div className="flex flex-col gap-6 p-6">
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold text-foreground text-balance">Painel de Licitações</h1>
            <p className="text-sm text-muted-foreground mt-1">Visão geral das licitações e concorrências</p>
          </div>
          <div className="flex flex-wrap items-center gap-2">
            <Button asChild size="sm">
              <Link href="/auctions/new">
                <Plus className="mr-2 h-4 w-4" />
                Nova Licitação
              </Link>
            </Button>
          </div>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
          <MetricCard
            title="Licitações Ativas"
            value={String(activeAuctions.length)}
            description="Em estruturação ou publicadas"
            icon={Gavel}
          />
          <MetricCard
            title="Licitações Ganhas"
            value={String(wonAuctions)}
            description="Total histórico de concorrências vencidas"
            icon={CheckCircle2}
          />
          <MetricCard
            title="Taxa de Sucesso"
            value={auctions?.length > 0 ? `${Math.round((wonAuctions / auctions.length) * 100)}%` : "0%"}
            description="Win rate global"
            icon={FileText}
          />
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <Card className="lg:col-span-2 border-border/60 overflow-hidden">
            <CardHeader className="pb-3 border-b border-border/40">
              <div className="flex items-center justify-between gap-2">
                <CardTitle className="text-base font-semibold">Licitações Recentes</CardTitle>
              </div>
            </CardHeader>
            <CardContent className="p-0">
              <div className="flex flex-col divide-y divide-border/40">
                {auctions && auctions.length > 0 ? (
                  auctions.slice(0, 5).map((auction: any) => {
                    const config = statusConfig[auction.status] || statusConfig[0]
                    return (
                      <div key={auction.id} className="p-4 hover:bg-accent/5 transition-colors group">
                        <div className="flex items-start justify-between gap-3">
                          <div className="flex flex-col gap-1 min-w-0">
                            <h4 className="text-sm font-semibold text-foreground truncate">{auction.title}</h4>
                            <p className="text-xs text-muted-foreground truncate">{auction.organization}</p>
                          </div>
                          <div className="flex flex-col items-end gap-2">
                            <span className={`text-[10px] px-2 py-1 rounded-full font-medium ${config.color}`}>
                              {config.label}
                            </span>
                          </div>
                        </div>
                      </div>
                    )
                  })
                ) : (
                  <div className="p-8 text-center flex flex-col items-center gap-2">
                    <Gavel className="h-8 w-8 text-muted-foreground/30" />
                    <p className="text-sm text-muted-foreground">Nenhuma licitação cadastrada ainda.</p>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>
        </div>

      </div>
    </AuthGuard>
  )
}

