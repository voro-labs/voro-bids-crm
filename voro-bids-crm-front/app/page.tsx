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
} from "lucide-react"
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Cell,
  PieChart,
  Pie,
  Legend,
  ResponsiveContainer,
} from "recharts"
import { ChartContainer, ChartTooltip, ChartTooltipContent, ChartLegend, ChartLegendContent } from "@/components/ui/chart"

import { API_CONFIG, secureApiCall } from "@/lib/api"
import { AuthGuard } from "@/components/auth/auth.guard"
import { format, parseISO } from "date-fns"
import { ptBR } from "date-fns/locale"

const fetcher = async (url: string) => {
  const result = await secureApiCall<any>(url, { method: "GET" })
  if (result.hasError) throw new Error(result.message || "Error")
  return result.data
}

const statusConfig: Record<number, { label: string; color: string; chartColor: string; icon: any }> = {
  0: { label: "Em Estruturação", color: "bg-gray-100 text-gray-800 border-gray-200", chartColor: "#6b7280", icon: Circle },
  1: { label: "Publicado", color: "bg-blue-100 text-blue-800 border-blue-200", chartColor: "#2563eb", icon: Clock },
  2: { label: "Suspenso", color: "bg-yellow-100 text-yellow-800 border-yellow-200", chartColor: "#d97706", icon: Circle },
  3: { label: "Cancelado", color: "bg-red-100 text-red-800 border-red-200", chartColor: "#dc2626", icon: XCircle },
  4: { label: "Encerrado", color: "bg-gray-200 text-gray-600 border-gray-300", chartColor: "#9ca3af", icon: CheckCircle2 },
  5: { label: "Ganha", color: "bg-green-100 text-green-800 border-green-200", chartColor: "#16a34a", icon: CheckCircle2 },
  6: { label: "Perdida", color: "bg-red-100 text-red-800 border-red-200", chartColor: "#ef4444", icon: XCircle },
}

// Always returns the last 6 months as fixed buckets, counting auctions per month
function buildMonthlyData(auctions: any[]): { month: string; total: number }[] {
  const now = new Date()
  // Build the 6-month window (oldest → newest)
  const months: { key: string; label: string }[] = []
  for (let i = 5; i >= 0; i--) {
    const d = new Date(now.getFullYear(), now.getMonth() - i, 1)
    months.push({
      key: `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}`,
      label: format(d, "MMM/yy", { locale: ptBR }),
    })
  }

  const counts: Record<string, number> = {}
  auctions.forEach((a) => {
    const raw = a.biddingDate || a.createdAt
    if (!raw) return
    try {
      const d = parseISO(raw)
      const key = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}`
      if (counts[key] !== undefined || months.some((m) => m.key === key)) {
        counts[key] = (counts[key] || 0) + 1
      }
    } catch { /* skip */ }
  })

  return months.map(({ key, label }) => ({ month: label, total: counts[key] || 0 }))
}

const barChartConfig = {
  total: { label: "Licitações", color: "hsl(var(--primary))" },
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
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <Card className="border-border/60"><CardContent className="p-6"><Skeleton className="h-56 w-full" /></CardContent></Card>
          <Card className="border-border/60"><CardContent className="p-6"><Skeleton className="h-56 w-full" /></CardContent></Card>
        </div>
      </div>
    )
  }

  const list: any[] = auctions || []
  const activeAuctions = list.filter((a) => [0, 1].includes(a.status))
  const wonAuctions = list.filter((a) => a.status === 5).length

  // Bar chart data — auctions per month
  const monthlyData = buildMonthlyData(list)

  // Pie chart data — auctions by status (only statuses that actually appear)
  const statusCounts: Record<number, number> = {}
  list.forEach((a) => { statusCounts[a.status] = (statusCounts[a.status] || 0) + 1 })
  const pieData = Object.entries(statusCounts).map(([status, value]) => ({
    name: statusConfig[Number(status)]?.label ?? `Status ${status}`,
    value,
    color: statusConfig[Number(status)]?.chartColor ?? "#8b5cf6",
  }))

  // Build a dynamic pie chart config for ChartContainer
  const pieChartConfig = Object.fromEntries(
    pieData.map((d) => [d.name, { label: d.name, color: d.color }])
  )

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

        {/* KPI cards */}
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
            value={list.length > 0 ? `${Math.round((wonAuctions / list.length) * 100)}%` : "0%"}
            description="Win rate global"
            icon={FileText}
          />
        </div>

        {/* Charts row */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">

          {/* Bar chart — licitações por mês */}
          <Card className="border-border/60">
            <CardHeader className="pb-2">
              <CardTitle className="text-base font-semibold">Licitações por Mês</CardTitle>
              <CardDescription className="text-xs">Quantidade de licitações por data de abertura</CardDescription>
            </CardHeader>
            <CardContent>
              {monthlyData.length === 0 ? (
                <div className="flex flex-col items-center justify-center h-48 gap-2 text-muted-foreground">
                  <Gavel className="h-8 w-8 opacity-30" />
                  <p className="text-sm">Sem dados suficientes</p>
                </div>
              ) : (
                <ChartContainer config={barChartConfig} className="h-56 w-full">
                  <BarChart data={monthlyData} margin={{ top: 4, right: 8, left: -16, bottom: 0 }}>
                    <CartesianGrid vertical={false} strokeDasharray="3 3" />
                    <XAxis
                      dataKey="month"
                      tickLine={false}
                      axisLine={false}
                      tick={{ fontSize: 11 }}
                    />
                    <YAxis tickLine={false} axisLine={false} tick={{ fontSize: 11 }} allowDecimals={false} />
                    <ChartTooltip content={<ChartTooltipContent nameKey="total" />} />
                    <Bar dataKey="total" fill="var(--color-total)" radius={[4, 4, 0, 0]} />
                  </BarChart>
                </ChartContainer>
              )}
            </CardContent>
          </Card>

          {/* Pie chart — licitações por situação */}
          <Card className="border-border/60">
            <CardHeader className="pb-2">
              <CardTitle className="text-base font-semibold">Licitações por Situação</CardTitle>
              <CardDescription className="text-xs">Distribuição de status das licitações cadastradas</CardDescription>
            </CardHeader>
            <CardContent>
              {pieData.length === 0 ? (
                <div className="flex flex-col items-center justify-center h-48 gap-2 text-muted-foreground">
                  <Gavel className="h-8 w-8 opacity-30" />
                  <p className="text-sm">Sem dados suficientes</p>
                </div>
              ) : (
                <ChartContainer config={pieChartConfig} className="h-56 w-full">
                  <PieChart>
                    <Pie
                      data={pieData}
                      cx="50%"
                      cy="50%"
                      innerRadius={55}
                      outerRadius={90}
                      paddingAngle={2}
                      dataKey="value"
                      nameKey="name"
                    >
                      {pieData.map((entry, idx) => (
                        <Cell key={`cell-${idx}`} fill={entry.color} stroke="transparent" />
                      ))}
                    </Pie>
                    <ChartTooltip
                      content={<ChartTooltipContent nameKey="name" hideLabel />}
                    />
                    <Legend
                      iconType="circle"
                      iconSize={8}
                      formatter={(value) => <span className="text-xs text-foreground">{value}</span>}
                    />
                  </PieChart>
                </ChartContainer>
              )}
            </CardContent>
          </Card>

        </div>

        {/* Recent auctions table */}
        <Card className="border-border/60 overflow-hidden">
          <CardHeader className="pb-3 border-b border-border/40">
            <div className="flex items-center justify-between gap-2">
              <CardTitle className="text-base font-semibold">Licitações Recentes</CardTitle>
              <Button asChild variant="ghost" size="sm" className="text-xs h-7">
                <Link href="/auctions">Ver todas</Link>
              </Button>
            </div>
          </CardHeader>
          <CardContent className="p-0">
            <div className="flex flex-col divide-y divide-border/40">
              {list.length > 0 ? (
                list.slice(0, 5).map((auction: any) => {
                  const config = statusConfig[auction.status] || statusConfig[0]
                  return (
                    <Link key={auction.id} href={`/auctions/${auction.id}`} className="p-4 hover:bg-accent/5 transition-colors group block">
                      <div className="flex items-start justify-between gap-3">
                        <div className="flex flex-col gap-1 min-w-0">
                          <h4 className="text-sm font-semibold text-foreground truncate">{auction.title}</h4>
                          <p className="text-xs text-muted-foreground truncate">{auction.organization}</p>
                        </div>
                        <span className={`text-[10px] px-2 py-1 rounded-full font-medium whitespace-nowrap shrink-0 ${config.color}`}>
                          {config.label}
                        </span>
                      </div>
                    </Link>
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
    </AuthGuard>
  )
}
