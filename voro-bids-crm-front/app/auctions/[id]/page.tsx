"use client"

import { useState, use } from "react"
import { secureApiCall, API_CONFIG } from "@/lib/api"
import useSWR from "swr"
import { useRouter } from "next/navigation"
import { ArrowLeft, FileText, Upload, Trash2, ShieldAlert } from "lucide-react"

import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle, CardFooter } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { useToast } from "@/hooks/use-toast"
import { Skeleton } from "@/components/ui/skeleton"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog"
import { Label } from "@/components/ui/label"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"

export interface Auction {
    id: string
    title: string
    organization: string
    biddingDate?: string
    status: number
}

export interface DocumentFileDto {
    id: string
    auctionDocumentId: string
    fileUrl: string
    fileName: string
    version: number
    uploadedById: string
    createdAt: string
}

export interface AuctionDocument {
    id: string
    auctionId: string
    name: string
    description?: string
    status: number
    requiresUpload: boolean
    files: DocumentFileDto[]
}

const fetchAuction = (url: string) => secureApiCall<Auction>(url).then((res) => res.data)
const fetchDocs = (url: string) => secureApiCall<AuctionDocument[]>(url).then((res) => res.data || [])

export default function AuctionDetailsPage({ params }: { params: Promise<{ id: string }> }) {
    const resolvedParams = use(params)
    const router = useRouter()
    const { toast } = useToast()
    const auctionId = resolvedParams.id

    const { data: auction, isLoading: isAuctionLoading } = useSWR<Auction>(
        `${API_CONFIG.ENDPOINTS.AUCTIONS}/${auctionId}`,
        fetchAuction
    )

    const { data: documents, isLoading: isDocsLoading, mutate: mutateDocs } = useSWR<AuctionDocument[]>(
        `${API_CONFIG.ENDPOINTS.AUCTIONS}/${auctionId}/documents`,
        fetchDocs
    )

    // Upload Requirement state
    const [isReqModalOpen, setIsReqModalOpen] = useState(false)
    const [isReqSubmitting, setIsReqSubmitting] = useState(false)
    const [reqFormData, setReqFormData] = useState({ name: "", description: "" })

    // Upload File state
    const [activeReqId, setActiveReqId] = useState<string | null>(null)
    const [isFileModalOpen, setIsFileModalOpen] = useState(false)
    const [isUploading, setIsUploading] = useState(false)
    const [selectedFile, setSelectedFile] = useState<File | null>(null)

    const handleCreateRequirement = async (e: React.FormEvent) => {
        e.preventDefault()
        if (!reqFormData.name) return

        setIsReqSubmitting(true)
        try {
            const res = await secureApiCall(`${API_CONFIG.ENDPOINTS.AUCTIONS}/${auctionId}/documents/requirements`, {
                method: "POST",
                body: JSON.stringify({
                    auctionId,
                    name: reqFormData.name,
                    description: reqFormData.description,
                    requiresUpload: true
                })
            })

            if (!res.hasError) {
                toast({ title: "Sucesso", description: "Requisito criado." })
                setIsReqModalOpen(false)
                setReqFormData({ name: "", description: "" })
                mutateDocs()
            } else {
                toast({ title: "Erro", description: res.message || "Erro ao criar", variant: "destructive" })
            }
        } catch {
            toast({ title: "Erro", description: "Falha na comunicação", variant: "destructive" })
        } finally {
            setIsReqSubmitting(false)
        }
    }

    const handleUploadFile = async (e: React.FormEvent) => {
        e.preventDefault()
        if (!selectedFile || !activeReqId) return

        setIsUploading(true)
        try {
            const formPayload = new FormData()
            formPayload.append("AuctionDocumentId", activeReqId)
            formPayload.append("File", selectedFile)

            const res = await secureApiCall(`${API_CONFIG.ENDPOINTS.AUCTIONS}/${auctionId}/documents/upload`, {
                method: "POST",
                body: formPayload
            })

            if (!res.hasError) {
                toast({ title: "Sucesso", description: "Arquivo enviado." })
                setIsFileModalOpen(false)
                setSelectedFile(null)
                setActiveReqId(null)
                mutateDocs()
            } else {
                toast({ title: "Erro", description: res.message || "Falha no upload.", variant: "destructive" })
            }
        } catch {
            toast({ title: "Erro", description: "Erro de rede.", variant: "destructive" })
        } finally {
            setIsUploading(false)
        }
    }

    const openDocument = async (url: string) => {
        try {
            const response = await fetch(`/api/blob/proxy?url=${encodeURIComponent(url)}`)
            const data = await response.json()

            if (response.ok && data.url) {
                window.open(data.url, "_blank")
            } else {
                toast({ title: "Erro", description: data.error || "Não foi possível resgatar o arquivo.", variant: "destructive" })
            }
        } catch {
            toast({ title: "Erro", description: "Falha no proxy.", variant: "destructive" })
        }
    }

    const deleteReq = async (id: string) => {
        if (!confirm("Tem certeza que deseja deletar este requisito e todos os seus arquivos?")) return
        try {
            const res = await secureApiCall(`${API_CONFIG.ENDPOINTS.AUCTIONS}/${auctionId}/documents/requirements/${id}`, { method: "DELETE" })
            if (!res.hasError) {
                toast({ title: "Sucesso", description: "Requisito excluído." })
                mutateDocs()
            }
        } catch {
            toast({ title: "Erro", description: "Erro ao excluir requisito.", variant: "destructive" })
        }
    }

    const deleteFile = async (fileId: string) => {
        if (!confirm("Deletar arquivo?")) return
        try {
            const res = await secureApiCall(`${API_CONFIG.ENDPOINTS.AUCTIONS}/${auctionId}/documents/files/${fileId}`, { method: "DELETE" })
            if (!res.hasError) {
                toast({ title: "Sucesso", description: "Arquivo excluído." })
                mutateDocs()
            }
        } catch {
            toast({ title: "Erro", description: "Erro ao excluir arquivo.", variant: "destructive" })
        }
    }

    if (isAuctionLoading) return <div className="p-8">Carregando...</div>

    return (
        <div className="flex-1 space-y-4 p-8 pt-6">
            <div className="flex items-center space-x-4 mb-4">
                <Button variant="outline" size="icon" onClick={() => router.push('/auctions')}>
                    <ArrowLeft className="h-4 w-4" />
                </Button>
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">{auction?.title || "Detalhes da Licitação"}</h2>
                    <p className="text-muted-foreground">{auction?.organization}</p>
                </div>
            </div>

            <Tabs defaultValue="documents" className="w-full">
                <TabsList>
                    <TabsTrigger value="overview">Visão Geral</TabsTrigger>
                    <TabsTrigger value="documents">Documentos</TabsTrigger>
                    <TabsTrigger value="tasks">Tarefas</TabsTrigger>
                </TabsList>
                <TabsContent value="overview">
                    <Card>
                        <CardHeader>
                            <CardTitle>Resumo da Licitação</CardTitle>
                        </CardHeader>
                        <CardContent>
                            Em desenvolvimento.
                        </CardContent>
                    </Card>
                </TabsContent>
                <TabsContent value="documents" className="space-y-4 pt-4">
                    {/* Upload Modals & Header */}
                    <div className="flex items-center justify-between">
                        <h3 className="text-lg font-medium">Requisitos e Arquivos</h3>
                        <Dialog open={isReqModalOpen} onOpenChange={setIsReqModalOpen}>
                            <DialogTrigger asChild>
                                <Button variant="default">
                                    Adicionar Requisito
                                </Button>
                            </DialogTrigger>
                            <DialogContent>
                                <DialogHeader>
                                    <DialogTitle>Novo Requisito Documental</DialogTitle>
                                    <DialogDescription>
                                        Adicione um item que precisa de comprovação via documento.
                                    </DialogDescription>
                                </DialogHeader>
                                <form onSubmit={handleCreateRequirement} className="space-y-4 pt-4">
                                    <div className="space-y-2">
                                        <Label>Nome</Label>
                                        <Input value={reqFormData.name} onChange={e => setReqFormData({ ...reqFormData, name: e.target.value })} required />
                                    </div>
                                    <div className="space-y-2">
                                        <Label>Descrição</Label>
                                        <Textarea value={reqFormData.description} onChange={e => setReqFormData({ ...reqFormData, description: e.target.value })} />
                                    </div>
                                    <DialogFooter>
                                        <Button type="submit" disabled={isReqSubmitting}>
                                            {isReqSubmitting ? "Salvando..." : "Salvar"}
                                        </Button>
                                    </DialogFooter>
                                </form>
                            </DialogContent>
                        </Dialog>
                    </div>

                    {/* Document File Uploader */}
                    <Dialog open={isFileModalOpen} onOpenChange={(open) => {
                        setIsFileModalOpen(open);
                        if (!open) { setActiveReqId(null); setSelectedFile(null); }
                    }}>
                        <DialogContent>
                            <DialogHeader>
                                <DialogTitle>Anexar Arquivo</DialogTitle>
                                <DialogDescription>Faça upload para comprovar o requisito.</DialogDescription>
                            </DialogHeader>
                            <form onSubmit={handleUploadFile} className="space-y-4 pt-4">
                                <div className="space-y-2">
                                    <Label>Selecionar Arquivo</Label>
                                    <Input type="file" onChange={e => setSelectedFile(e.target.files?.[0] || null)} required />
                                </div>
                                <DialogFooter>
                                    <Button type="submit" disabled={isUploading}>
                                        {isUploading ? "Enviando..." : "Enviar Arquivo"}
                                    </Button>
                                </DialogFooter>
                            </form>
                        </DialogContent>
                    </Dialog>

                    {isDocsLoading ? (
                        <Skeleton className="h-40 w-full" />
                    ) : documents?.length === 0 ? (
                        <div className="border border-dashed rounded-lg p-8 text-center bg-muted/20">
                            <p className="text-muted-foreground">Nenhum requisito documental cadastrado para essa licitação.</p>
                        </div>
                    ) : (
                        <div className="space-y-4">
                            {documents?.map(doc => (
                                <Card key={doc.id}>
                                    <CardHeader className="flex flex-row items-start justify-between">
                                        <div>
                                            <CardTitle className="text-base flex items-center gap-2">
                                                {doc.status === 1 ? <ShieldAlert className="h-4 w-4 text-orange-500" /> : <FileText className="h-4 w-4 text-green-500" />}
                                                {doc.name}
                                            </CardTitle>
                                            {doc.description && <CardDescription>{doc.description}</CardDescription>}
                                        </div>
                                        <div className="flex gap-2">
                                            <Button variant="outline" size="sm" onClick={() => { setActiveReqId(doc.id); setIsFileModalOpen(true); }}>
                                                <Upload className="h-4 w-4 mr-2" />
                                                Anexar
                                            </Button>
                                            <Button variant="ghost" size="sm" className="text-destructive" onClick={() => deleteReq(doc.id)}>
                                                <Trash2 className="h-4 w-4" />
                                            </Button>
                                        </div>
                                    </CardHeader>
                                    {doc.files.length > 0 && (
                                        <CardContent>
                                            <div className="bg-muted/30 rounded-md py-2 border">
                                                {doc.files.map(file => (
                                                    <div key={file.id} className="flex items-center justify-between px-4 py-2 hover:bg-muted/50 transition-colors border-b last:border-0">
                                                        <div className="flex items-center gap-3">
                                                            <span className="text-sm font-medium">{file.fileName}</span>
                                                            <Badge variant="secondary" className="text-[10px]">v{file.version}</Badge>
                                                        </div>
                                                        <div className="flex gap-2">
                                                            <Button variant="ghost" size="sm" onClick={() => openDocument(file.fileUrl)}>Visualizar</Button>
                                                            <Button variant="ghost" size="sm" className="text-destructive px-2" onClick={() => deleteFile(file.id)}>
                                                                <Trash2 className="h-3 w-3" />
                                                            </Button>
                                                        </div>
                                                    </div>
                                                ))}
                                            </div>
                                        </CardContent>
                                    )}
                                </Card>
                            ))}
                        </div>
                    )}
                </TabsContent>
            </Tabs>
        </div>
    )
}
