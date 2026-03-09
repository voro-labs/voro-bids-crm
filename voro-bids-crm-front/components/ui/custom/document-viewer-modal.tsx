"use client"

import { useState, useEffect } from "react"
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Loader2, Download, X, FileText, Image as ImageIcon } from "lucide-react"

interface DocumentViewerModalProps {
  open: boolean
  onClose: () => void
  url: string | null
  fileName?: string
}

function detectType(blob: Blob, url: string): "pdf" | "image" | "other" {
  if (blob.type === "application/pdf" || url.toLowerCase().includes(".pdf")) return "pdf"
  if (blob.type.startsWith("image/") || /\.(png|jpe?g|gif|webp|svg|bmp)$/i.test(url)) return "image"
  return "other"
}

export function DocumentViewerModal({ open, onClose, url, fileName }: DocumentViewerModalProps) {
  const [blobUrl, setBlobUrl] = useState<string | null>(null)
  const [fileType, setFileType] = useState<"pdf" | "image" | "other" | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!open || !url) return

    let objectUrl: string | null = null
    setLoading(true)
    setError(null)
    setBlobUrl(null)
    setFileType(null)

    const fetchDoc = async () => {
      try {
        const response = await fetch(`/api/blob/proxy?url=${encodeURIComponent(url)}`)
        if (!response.ok) throw new Error("Falha ao carregar documento")

        const blob = await response.blob()
        objectUrl = URL.createObjectURL(blob)
        setBlobUrl(objectUrl)
        setFileType(detectType(blob, url))
      } catch (e: any) {
        setError(e.message || "Erro ao carregar o arquivo.")
      } finally {
        setLoading(false)
      }
    }

    fetchDoc()

    return () => {
      if (objectUrl) URL.revokeObjectURL(objectUrl)
    }
  }, [open, url])

  const handleDownload = () => {
    if (!blobUrl) return
    const a = document.createElement("a")
    a.href = blobUrl
    a.download = fileName || "documento"
    a.click()
  }

  return (
    <Dialog open={open} onOpenChange={(v) => !v && onClose()}>
      <DialogContent className="max-w-4xl w-full h-[90vh] flex flex-col p-0 gap-0" showCloseButton={false}>
        <DialogHeader className="flex flex-row items-center justify-between px-4 py-3 border-b shrink-0">
          <div className="flex items-center gap-2 min-w-0 flex-1">
            {fileType === "pdf" ? (
              <FileText className="h-4 w-4 text-primary shrink-0" />
            ) : (
              <ImageIcon className="h-4 w-4 text-primary shrink-0" />
            )}
            <DialogTitle className="text-sm font-medium truncate">
              {fileName || "Visualizar Documento"}
            </DialogTitle>
          </div>
          <div className="flex items-center gap-2 shrink-0">
            {blobUrl && (
              <Button variant="outline" size="sm" onClick={handleDownload} className="h-8">
                <Download className="h-3.5 w-3.5 mr-1.5" />
                Baixar
              </Button>
            )}
            <Button
              variant="ghost"
              size="icon"
              onClick={onClose}
              className="h-8 w-8 text-muted-foreground hover:text-foreground"
              aria-label="Fechar"
            >
              <X className="h-4 w-4" />
            </Button>
          </div>
        </DialogHeader>

        <div className="flex-1 overflow-hidden bg-muted/30 relative">
          {loading && (
            <div className="absolute inset-0 flex flex-col items-center justify-center gap-3">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
              <p className="text-sm text-muted-foreground">Carregando documento...</p>
            </div>
          )}

          {error && (
            <div className="absolute inset-0 flex flex-col items-center justify-center gap-3 p-6 text-center">
              <FileText className="h-12 w-12 text-muted-foreground/50" />
              <p className="text-sm text-destructive font-medium">{error}</p>
              <p className="text-xs text-muted-foreground">Tente baixar o arquivo diretamente.</p>
            </div>
          )}

          {!loading && !error && blobUrl && fileType === "pdf" && (
            <iframe
              src={blobUrl}
              className="w-full h-full border-0"
              title={fileName || "Documento PDF"}
            />
          )}

          {!loading && !error && blobUrl && fileType === "image" && (
            <div className="w-full h-full flex items-center justify-center p-4 overflow-auto">
              <img
                src={blobUrl}
                alt={fileName || "Imagem"}
                className="max-w-full max-h-full object-contain rounded-md shadow"
              />
            </div>
          )}

          {!loading && !error && blobUrl && fileType === "other" && (
            <div className="absolute inset-0 flex flex-col items-center justify-center gap-4 p-6 text-center">
              <FileText className="h-16 w-16 text-muted-foreground/40" />
              <p className="text-sm text-muted-foreground">
                Pré-visualização não disponível para este tipo de arquivo.
              </p>
              <Button onClick={handleDownload}>
                <Download className="h-4 w-4 mr-2" />
                Baixar Arquivo
              </Button>
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  )
}
