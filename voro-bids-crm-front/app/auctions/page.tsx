"use client"

import { useState } from "react"
import useSWR from "swr"
import Link from "next/link"
import { format } from "date-fns"
import { ptBR } from "date-fns/locale"
import { Plus, Search, Gavel, Calendar, Edit, Eye, Filter } from "lucide-react"

import { API_CONFIG, secureApiCall } from "@/lib/api"
import { AuthGuard } from "@/components/auth/auth.guard"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Badge } from "@/components/ui/badge"
import { Skeleton } from "@/components/ui/skeleton"
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card"

const fetcher = async (url: string) => {
    const result = await secureApiCall<any>(url, { method: "GET" })
    if (result.hasError) throw new Error(result.message || "Erro ao carregar")
    return result.data
}

// Bidding status config (0: Setup, 1: Published, 2: Suspended, 3: Canceled, 4: Finished, 5: Won, 6: Lost)
const statusConfig: Record<number, { label: string; color: string; icon: any }> = {
    0: { label: "Em Estruturação", color: "bg-gray-100 text-gray-800 border-gray-200", icon: Gavel },
    1: { label: "Publicado", color: "bg-blue-100 text-blue-800 border-blue-200", icon: Calendar },
    2: { label: "Suspenso", color: "bg-yellow-100 text-yellow-800 border-yellow-200", icon: Gavel },
    3: { label: "Cancelado", color: "bg-red-100 text-red-800 border-red-200", icon: Gavel },
    4: { label: "Encerrado", color: "bg-gray-200 text-gray-600 border-gray-300", icon: Gavel },
    5: { label: "Ganha", color: "bg-green-100 text-green-800 border-green-200", icon: Gavel },
    6: { label: "Perdida", color: "bg-red-100 text-red-800 border-red-200", icon: Gavel },
}

export default function AuctionsPage() {
    const [searchTerm, setSearchTerm] = useState("")
    const [statusFilter, setStatusFilter] = useState("all")

    const { data: auctions, isLoading, error } = useSWR(API_CONFIG.ENDPOINTS.AUCTIONS, fetcher)

    const filteredAuctions = auctions?.filter((auction: any) => {
        const matchesSearch = auction.title?.toLowerCase().includes(searchTerm.toLowerCase()) ||
            auction.organization?.toLowerCase().includes(searchTerm.toLowerCase());
        const matchesStatus = statusFilter === "all" || auction.status.toString() === statusFilter;

        return matchesSearch && matchesStatus;
    }) || [];

    return (
        <AuthGuard requiredRoles={["Admin", "User", "Legal", "Finance", "Management", "Operational"]}>
            <div className="flex flex-col gap-6 p-6">
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                    <div>
                        <h1 className="text-2xl font-bold text-foreground">Licitações</h1>
                        <p className="text-sm text-muted-foreground mt-1">
                            Gerencie suas concorrências e licitações ativas.
                        </p>
                    </div>
                    <Button asChild>
                        <Link href="/auctions/new" className="flex items-center gap-2">
                            <Plus className="h-4 w-4" />
                            Nova Licitação
                        </Link>
                    </Button>
                </div>

                <div className="flex flex-col sm:flex-row gap-4 items-center justify-between bg-card p-4 rounded-xl border border-border mt-2 shadow-sm">
                    <div className="relative w-full sm:w-96">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground h-4 w-4" />
                        <Input
                            placeholder="Buscar por título ou órgão..."
                            className="pl-9 bg-background"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>

                    <div className="w-full sm:w-auto flex items-center gap-2 overflow-x-auto pb-1 sm:pb-0 hide-scrollbar">
                        <Button
                            variant={statusFilter === "all" ? "default" : "outline"}
                            size="sm"
                            onClick={() => setStatusFilter("all")}
                        >
                            Todas
                        </Button>
                        <Button
                            variant={statusFilter === "1" ? "default" : "outline"}
                            size="sm"
                            onClick={() => setStatusFilter("1")}
                        >
                            Publicadas
                        </Button>
                        <Button
                            variant={statusFilter === "0" ? "default" : "outline"}
                            size="sm"
                            onClick={() => setStatusFilter("0")}
                        >
                            Em Estruturação
                        </Button>
                    </div>
                </div>

                {isLoading ? (
                    <div className="space-y-4">
                        {[1, 2, 3].map(i => (
                            <Card key={i} className="border-border">
                                <CardHeader className="py-4">
                                    <Skeleton className="h-6 w-1/3 mb-2" />
                                    <Skeleton className="h-4 w-1/4" />
                                </CardHeader>
                            </Card>
                        ))}
                    </div>
                ) : error ? (
                    <div className="p-8 text-center bg-card rounded-xl border border-destructive/20 text-destructive">
                        <p>Erro ao carregar licitações. Tente novamente.</p>
                    </div>
                ) : filteredAuctions.length === 0 ? (
                    <div className="flex flex-col items-center justify-center p-12 text-center bg-card rounded-xl border border-border border-dashed">
                        <div className="h-16 w-16 bg-muted/50 rounded-full flex items-center justify-center mb-4">
                            <Gavel className="h-8 w-8 text-muted-foreground" />
                        </div>
                        <h3 className="text-lg font-medium text-foreground">Ainda não há licitações</h3>
                        <p className="text-sm text-muted-foreground max-w-sm mt-1 mb-6">
                            Nenhuma licitação encontrada. Clique no botão abaixo para criar a sua primeira.
                        </p>
                        <Button asChild variant="outline">
                            <Link href="/auctions/new">
                                <Plus className="mr-2 h-4 w-4" />
                                Nova Licitação
                            </Link>
                        </Button>
                    </div>
                ) : (
                    <div className="grid grid-cols-1 gap-4">
                        {filteredAuctions.map((auction: any) => {
                            const config = statusConfig[auction.status] || statusConfig[0];
                            return (
                                <Card key={auction.id} className="transition-all hover:border-primary/50 group">
                                    <CardHeader className="flex flex-col sm:flex-row sm:items-start justify-between gap-4 py-4">
                                        <div className="space-y-1.5 flex-1">
                                            <div className="flex items-center gap-2 mb-1">
                                                <Badge variant="outline" className={`font-mono ${config.color} border-none`}>
                                                    {config.label}
                                                </Badge>
                                                <span className="text-[10px] text-muted-foreground bg-muted px-2 py-0.5 rounded-full">
                                                    Criado em {format(new Date(auction.createdAt), "dd/MM/yyyy", { locale: ptBR })}
                                                </span>
                                            </div>
                                            <CardTitle className="text-xl font-semibold leading-tight">{auction.title}</CardTitle>
                                            <CardDescription className="text-sm font-medium flex items-center gap-1.5 text-foreground/80">
                                                <Gavel className="h-3.5 w-3.5" />
                                                {auction.organization}
                                            </CardDescription>
                                        </div>
                                        <div className="flex items-center gap-2 shrink-0">
                                            <Button variant="outline" size="sm" className="hidden sm:flex" asChild>
                                                <Link href={`/auctions/${auction.id}`}>
                                                    <Eye className="mr-2 h-4 w-4" /> Detalhes
                                                </Link>
                                            </Button>
                                            <Button variant="outline" size="icon" asChild>
                                                <Link href={`/auctions/${auction.id}/edit`}>
                                                    <Edit className="h-4 w-4" />
                                                </Link>
                                            </Button>
                                        </div>
                                    </CardHeader>
                                </Card>
                            );
                        })}
                    </div>
                )}
            </div>
        </AuthGuard>
    )
}
