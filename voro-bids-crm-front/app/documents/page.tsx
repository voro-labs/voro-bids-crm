"use client"

import { useState } from "react"
import { secureApiCall, API_CONFIG } from "@/lib/api"
import useSWR from "swr"
import { FileText, Plus, Download, Trash2, Calendar, LayoutDashboard, Pencil } from "lucide-react"

import { DocumentViewerModal } from "@/components/ui/custom/document-viewer-modal"

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
  const [isUploadModalOpen, setIsUploadModalOpen] = useState(false)
  const [isUploading, setIsUploading] = useState(false)

  // Viewer state
  const [isViewerOpen, setIsViewerOpen] = useState(false)
  const [viewerUrl, setViewerUrl] = useState<string | null>(null)
  const [viewerFileName, setViewerFileName] = useState<string | undefined>(undefined)

  const [formData, setFormData] = useState({
    name: "",
    description: "",
    expirationDate: "",
    file: null as File | null,
  })

  // Edit state
  const [isEditModalOpen, setIsEditModalOpen] = useState(false)
  const [editingDoc, setEditingDoc] = useState<CompanyDocument | null>(null)
  const [isEditing, setIsEditing] = useState(false)
  const [editFormData, setEditFormData] = useState({
    name: "",
    description: "",
    expirationDate: "",
  })

  const filteredDocs = documents?.filter(
    (doc) =>
      doc.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      (doc.description && doc.description.toLowerCase().includes(searchQuery.toLowerCase()))
  )

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      setFormData({ ...formData, file: e.target.files[0] })
    }
  }

  const handleUploadSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!formData.file) {
      toast({ title: "Erro", description: "Selecione um arquivo", variant: "destructive" })
      return
    }
    if (!formData.name) {
      toast({ title: "Erro", description: "Dê um nome ao documento", variant: "destructive" })
      return
    }

    setIsUploading(true)

    try {
      const formPayload = new FormData()
      formPayload.append("Name", formData.name)
      if (formData.description) formPayload.append("Description", formData.description)
      if (formData.expirationDate) {
        const isoDate = new Date(formData.expirationDate).toISOString()
        formPayload.append("ExpirationDate", isoDate)
      }
      formPayload.append("File", formData.file)

      const res = await secureApiCall(API_CONFIG.ENDPOINTS.COMPANY_DOCUMENTS, {
        method: "POST",
        body: formPayload,
      })

      if (!res.hasError) {
        toast({ title: "Sucesso", description: "Documento enviado." })
        setIsUploadModalOpen(false)
        setFormData({ name: "", description: "", expirationDate: "", file: null })
        mutate()
      } else {
        toast({ title: "Erro", description: res.message || "Falha no upload.", variant: "destructive" })
      }
    } catch (err) {
      toast({ title: "Erro", description: "Falha na comunicação com o servidor.", variant: "destructive" })
    } finally {
      setIsUploading(false)
    }
  }

  const openDocument = (url: string, name?: string) => {
    setViewerUrl(url)
    setViewerFileName(name)
    setIsViewerOpen(true)
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

  const openEditModal = (doc: CompanyDocument) => {
    setEditingDoc(doc)
    setEditFormData({
      name: doc.name,
      description: doc.description || "",
      expirationDate: doc.expirationDate
        ? new Date(doc.expirationDate).toISOString().slice(0, 10)
        : "",
    })
    setIsEditModalOpen(true)
  }

  const handleEditSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!editingDoc || !editFormData.name.trim()) return

    setIsEditing(true)
    try {
      const payload = {
        name: editFormData.name.trim(),
        description: editFormData.description || null,
        expirationDate: editFormData.expirationDate
          ? new Date(editFormData.expirationDate).toISOString()
          : null,
      }

      const res = await secureApiCall(`${API_CONFIG.ENDPOINTS.COMPANY_DOCUMENTS}/${editingDoc.id}`, {
        method: "PATCH",
        body: JSON.stringify(payload),
      })

      if (!res.hasError) {
        toast({ title: "Sucesso", description: "Documento atualizado." })
        setIsEditModalOpen(false)
        setEditingDoc(null)
        mutate()
      } else {
        toast({ title: "Erro", description: res.message || "Erro ao atualizar documento.", variant: "destructive" })
      }
    } catch {
      toast({ title: "Erro", description: "Falha na comunicação com o servidor.", variant: "destructive" })
    } finally {
      setIsEditing(false)
    }
  }

  return (
    <div className="flex-1 space-y-4 p-4 sm:p-8 pt-4 sm:pt-6">
      <DocumentViewerModal
        open={isViewerOpen}
        onClose={() => setIsViewerOpen(false)}
        url={viewerUrl}
        fileName={viewerFileName}
      />
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
        <div>
          <h2 className="text-2xl sm:text-3xl font-bold tracking-tight">Documentos Institucionais</h2>
          <p className="text-muted-foreground text-sm mt-0.5">
            Gerencie certidões, alvarás e atestados essenciais para licitações.
          </p>
        </div>
        <div className="flex items-center">
          <Dialog open={isUploadModalOpen} onOpenChange={setIsUploadModalOpen}>
            <DialogTrigger asChild>
              <Button>
                <Plus className="mr-2 h-4 w-4" />
                Novo Documento
              </Button>
            </DialogTrigger>
            <DialogContent className="sm:max-w-[425px]">
              <DialogHeader>
                <DialogTitle>Enviar Documento</DialogTitle>
                <DialogDescription>
                  Faça o upload de uma certidão ou atestado. Documentos ficam disponíveis para uso em licitações.
                </DialogDescription>
              </DialogHeader>
              <form onSubmit={handleUploadSubmit} className="space-y-4 pt-4">
                <div className="space-y-2">
                  <Label htmlFor="name">Nome do Documento <span className="text-red-500">*</span></Label>
                  <Input
                    id="name"
                    value={formData.name}
                    onChange={e => setFormData({ ...formData, name: e.target.value })}
                    placeholder="Ex: Certidão Negativa de Débitos Federais"
                    required
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="description">Descrição</Label>
                  <Textarea
                    id="description"
                    value={formData.description}
                    onChange={e => setFormData({ ...formData, description: e.target.value })}
                    placeholder="Informações adicionais (opcional)"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="expirationDate">Data de Validade/Expiração</Label>
                  <Input
                    id="expirationDate"
                    type="date"
                    value={formData.expirationDate}
                    onChange={e => setFormData({ ...formData, expirationDate: e.target.value })}
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="file">Arquivo (PDF, Imagens) <span className="text-red-500">*</span></Label>
                  <Input
                    id="file"
                    type="file"
                    onChange={handleFileChange}
                    required
                  />
                </div>
                <DialogFooter className="pt-4">
                  <Button type="button" variant="outline" onClick={() => setIsUploadModalOpen(false)}>Cancelar</Button>
                  <Button type="submit" disabled={isUploading}>
                    {isUploading ? "Enviando..." : "Salvar Documento"}
                  </Button>
                </DialogFooter>
              </form>
            </DialogContent>
          </Dialog>
        </div>
      </div>

      {/* Edit Document Dialog */}
      <Dialog open={isEditModalOpen} onOpenChange={setIsEditModalOpen}>
        <DialogContent className="sm:max-w-[425px]">
          <DialogHeader>
            <DialogTitle>Editar Documento</DialogTitle>
            <DialogDescription>
              Atualize os dados do documento. O arquivo não será alterado.
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={handleEditSubmit} className="space-y-4 pt-4">
            <div className="space-y-2">
              <Label htmlFor="edit-name">Nome do Documento <span className="text-red-500">*</span></Label>
              <Input
                id="edit-name"
                value={editFormData.name}
                onChange={e => setEditFormData({ ...editFormData, name: e.target.value })}
                placeholder="Ex: Certidão Negativa de Débitos Federais"
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="edit-description">Descrição</Label>
              <Textarea
                id="edit-description"
                value={editFormData.description}
                onChange={e => setEditFormData({ ...editFormData, description: e.target.value })}
                placeholder="Informações adicionais (opcional)"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="edit-expirationDate">Data de Validade/Expiração</Label>
              <Input
                id="edit-expirationDate"
                type="date"
                value={editFormData.expirationDate}
                onChange={e => setEditFormData({ ...editFormData, expirationDate: e.target.value })}
              />
            </div>
            <DialogFooter className="pt-4">
              <Button type="button" variant="outline" onClick={() => setIsEditModalOpen(false)}>Cancelar</Button>
              <Button type="submit" disabled={isEditing}>
                {isEditing ? "Salvando..." : "Salvar Alterações"}
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      <div className="flex items-center mb-4">
        <Input
          placeholder="Buscar documento por nome ou descrição..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="w-full max-w-sm"
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
        <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3">
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
                    <Badge
                      className={`mt-2 font-normal flex items-center gap-1 w-fit border-transparent text-white ${isExpired ? 'bg-destructive hover:bg-destructive/90' : 'bg-green-600 hover:bg-green-700'}`}
                    >
                      <Calendar className="h-3 w-3" />
                      {isExpired ? "Expirado em " : "Válido até "}
                      {format(new Date(doc.expirationDate), "dd/MM/yyyy", { locale: ptBR })}
                    </Badge>
                  ) : (
                    <Badge variant="outline" className="mt-2 font-normal text-muted-foreground">Documento Sem Validade</Badge>
                  )}
                </CardContent>
                <CardFooter className="flex justify-between items-center bg-muted/50 py-3 rounded-b-lg border-t">
                  <Button variant="ghost" size="sm" onClick={() => openDocument(doc.fileUrl, doc.name)}>
                    <Download className="mr-2 h-4 w-4" />
                    Visualizar
                  </Button>
                  <div className="flex items-center gap-1">
                    <Button variant="ghost" size="sm" className="h-8 w-8 p-0" onClick={() => openEditModal(doc)}>
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button variant="ghost" size="sm" className="text-destructive hover:bg-destructive/10 h-8 w-8 p-0" onClick={() => deleteDocument(doc.id)}>
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                </CardFooter>
              </Card>
            )
          })}
        </div>
      )}
    </div>
  )
}
