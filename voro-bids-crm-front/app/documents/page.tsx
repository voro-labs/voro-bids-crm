"use client"

import { useState } from "react"
import { secureApiCall, API_CONFIG } from "@/lib/api"
import useSWR from "swr"
import { FileText, Plus, Download, Trash2, Calendar, LayoutDashboard } from "lucide-react"

import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle, CardFooter } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Badge } from "@/components/ui/badge"
import { useToast } from "@/hooks/use-toast"
import { Skeleton } from "@/components/ui/skeleton"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { format } from "date-fns"
import { ptBR } from "date-fns/locale"

export interface CompanyDocument {
    id: string
    name: string
    description?: string
    fileUrl: string
    expirationDate?: string
    createdAt: string
}

const fetcher = (url: string) => secureApiCall<CompanyDocument[]>(url).then((res) => res.data || [])

export default function DocumentsPage() {
    const { toast } = useToast()
    const { data: documents, error, isLoading, mutate } = useSWR<CompanyDocument[]>(
        API_CONFIG.ENDPOINTS.COMPANY_DOCUMENTS,
        fetcher
    )

    const [searchQuery, setSearchQuery] = useState("")

    const filteredDocs = documents?.filter(
        (doc) =>
            doc.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
            (doc.description && doc.description.toLowerCase().includes(searchQuery.toLowerCase()))
    )

    const openDocument = async (url: string) => {
        try {
            // Usamos nosso proxy /api/blob/proxy para acessar blobs privados via Vercel
            const response = await fetch(`/api/blob/proxy?url=${encodeURIComponent(url)}`)
            const data = await response.json()

            if (response.ok && data.url) {
                window.open(data.url, "_blank")
            } else {
                toast({ title: "Erro", description: data.error || "Não foi possível resgatar o arquivo.", variant: "destructive" })
            }
        } catch (error) {
            toast({ title: "Erro", description: "Falha na comunicação com o proxy do documento.", variant: "destructive" })
        }
    }

    const deleteDocument = async (id: string) => {
        if (!confirm("Tem certeza que deseja deletar este documento?")) return

        try {
            const res = await secureApiCall(`${API_CONFIG.ENDPOINTS.COMPANY_DOCUMENTS}/${id}`, {
                method: "DELETE"
            })

            if (!res.hasError) {
                toast({ title: "Sucesso", description: "Documento deletado." })
                mutate()
            } else {
                toast({ title: "Erro", description: res.message || "Erro ao deletar documento", variant: "destructive" })
            }
        } catch (e) {
            toast({ title: "Erro", description: "Falha na rede ao deletar.", variant: "destructive" })
        }
    }

    return (
        <div className="flex-1 space-y-4 p-8 pt-6">
            <div className="flex items-center justify-between space-y-2">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Documentos Institucionais</h2>
                    <p className="text-muted-foreground">
                        Gerencie certidões, alvarás e atestados essenciais para licitações.
                    </p>
                </div>
                <div className="flex items-center space-x-2">
                    {/* O botão New será resolvido em seguida */}
                    <Button onClick={() => alert("Modal de upload de documento em desenvolvimento.")}>
                        <Plus className="mr-2 h-4 w-4" />
                        Novo Documento
                    </Button>
                </div>
            </div>

            <div className="flex items-center space-x-2 mb-6">
                <Input
                    placeholder="Buscar documento por nome ou descrição..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="max-w-sm"
                />
            </div>

            {isLoading ? (
                <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                    {[1, 2, 3].map((i) => (
                        <Card key={i}>
                            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                                <Skeleton className="h-4 w-[150px]" />
                                <Skeleton className="h-4 w-4" />
                            </CardHeader>
                            <CardContent>
                                <Skeleton className="h-4 w-full mb-2" />
                                <Skeleton className="h-4 w-[100px]" />
                            </CardContent>
                        </Card>
                    ))}
                </div>
            ) : error ? (
                <div className="flex flex-col items-center py-12 text-center text-red-500">
                    Carregamento falhou: {error.message}
                </div>
            ) : filteredDocs?.length === 0 ? (
                <div className="flex flex-col items-center justify-center rounded-md border border-dashed p-8 text-center animate-in fade-in-50">
                    <div className="flex h-20 w-20 items-center justify-center rounded-full bg-muted/50 mb-4">
                        <LayoutDashboard className="h-10 w-10 text-muted-foreground" />
                    </div>
                    <h3 className="mb-2 text-xl font-semibold">Nenhum documento encontrado.</h3>
                    <p className="mb-4 text-sm text-muted-foreground max-w-sm">
                        Você ainda não enviou nenhum documento institucional ou as buscas não retornaram resultados.
                    </p>
                </div>
            ) : (
                <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                    {filteredDocs?.map((doc) => {
                        const isExpired = doc.expirationDate && new Date(doc.expirationDate) < new Date()
                        return (
                            <Card key={doc.id} className="group relative break-inside-avoid">
                                <CardHeader className="flex flex-row items-start justify-between space-y-0 pb-2">
                                    <div className="space-y-1">
                                        <CardTitle className="leading-snug">
                                            <div className="flex items-center gap-2">
                                                <FileText className="h-4 w-4 text-primary shrink-0" />
                                                <span className="line-clamp-2" title={doc.name}>{doc.name}</span>
                                            </div>
                                        </CardTitle>
                                        {doc.description && (
                                            <CardDescription className="line-clamp-2" title={doc.description}>
                                                {doc.description}
                                            </CardDescription>
                                        )}
                                    </div>
                                </CardHeader>
                                <CardContent className="pb-4">
                                    {doc.expirationDate ? (
                                        <Badge variant={isExpired ? "destructive" : "secondary"} className="mt-2 font-normal flex items-center gap-1 w-fit">
                                            <Calendar className="h-3 w-3" />
                                            {isExpired ? "Expirado em " : "Válido até "}
                                            {format(new Date(doc.expirationDate), "dd/MM/yyyy", { locale: ptBR })}
                                        </Badge>
                                    ) : (
                                        <Badge variant="outline" className="mt-2 font-normal text-muted-foreground">Documento Sem Validade</Badge>
                                    )}
                                </CardContent>
                                <CardFooter className="flex justify-between items-center bg-muted/50 py-3 rounded-b-lg border-t">
                                    <Button variant="ghost" size="sm" onClick={() => openDocument(doc.fileUrl)}>
                                        <Download className="mr-2 h-4 w-4" />
                                        Visualizar
                                    </Button>
                                    <Button variant="ghost" size="sm" className="text-destructive hover:bg-destructive/10" onClick={() => deleteDocument(doc.id)}>
                                        <Trash2 className="h-4 w-4" />
                                    </Button>
                                </CardFooter>
                            </Card>
                        )
                    })}
                </div>
            )}
        </div>
    )
}
