"use client"

import { useState, use, useEffect } from "react"
import { useRouter } from "next/navigation"
import Link from "next/link"
import { ArrowLeft, Save, Loader2 } from "lucide-react"
import { toast } from "sonner"
import useSWR from "swr"

import { API_CONFIG, secureApiCall } from "@/lib/api"
import { AuthGuard } from "@/components/auth/auth.guard"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"
import { Auction } from "../page"

const fetchAuction = (url: string) => secureApiCall<Auction>(url).then((res) => res.data as Auction | null)

export default function EditAuctionPage({ params }: { params: Promise<{ id: string }> }) {
  const resolvedParams = use(params)
  const router = useRouter()
  const auctionId = resolvedParams.id

  const { data: auction, isLoading, error } = useSWR<Auction | null>(
    `${API_CONFIG.ENDPOINTS.AUCTIONS}/${auctionId}`,
    fetchAuction
  )

  const [isSubmitting, setIsSubmitting] = useState(false)
  const [formData, setFormData] = useState({
    id: "",
    title: "",
    organization: "",
    biddingDate: "",
    publishDate: "",
    status: 0,
  })

  useEffect(() => {
    if (auction) {
      setFormData({
        id: auction.id || "",
        title: auction.title || "",
        organization: auction.organization || "",
        biddingDate: auction.biddingDate ? new Date(auction.biddingDate).toISOString().slice(0, 16) : "",
        // @ts-ignore - publishDate might not be in the Auction interface yet, but it's submitted in the new page
        publishDate: auction.publishDate ? new Date(auction.publishDate).toISOString().slice(0, 10) : "",
        status: auction.status || 0,
      })
    }
  }, [auction])

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target
    setFormData(prev => ({ ...prev, [name]: value }))
  }

  const handleStatusChange = (value: string) => {
    setFormData(prev => ({ ...prev, status: parseInt(value, 10) }))
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsSubmitting(true)

    try {
      // Format dates
      const payload = {
        id: formData.id,
        title: formData.title,
        organization: formData.organization,
        status: formData.status,
        biddingDate: formData.biddingDate ? new Date(formData.biddingDate).toISOString() : null,
        publishDate: formData.publishDate ? new Date(formData.publishDate).toISOString() : null,
      }

      const res = await secureApiCall(`${API_CONFIG.ENDPOINTS.AUCTIONS}/${auctionId}`, {
        method: "PUT",
        body: JSON.stringify(payload),
      })

      if (res.hasError) {
        toast.error(res.message || "Erro ao atualizar licitação.")
      } else {
        toast.success("Licitação atualizada com sucesso!")
        router.push(`/auctions/${auctionId}`)
        router.refresh()
      }
    } catch (error) {
      toast.error("Erro inesperado.")
      console.error(error)
    } finally {
      setIsSubmitting(false)
    }
  }

  if (error) {
    return (
      <div className="flex flex-col items-center justify-center p-8">
        <p className="text-destructive mb-4">Erro ao carregar os dados da licitação.</p>
        <Button variant="outline" onClick={() => router.push("/auctions")}>Voltar para a lista</Button>
      </div>
    )
  }

  return (
    <AuthGuard requiredRoles={["Admin", "User", "Legal", "Finance", "Management", "Operational"]}>
      <div className="flex flex-col gap-6 p-6 max-w-3xl mx-auto w-full">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" asChild>
            <Link href={`/auctions/${auctionId}`}>
              <ArrowLeft className="h-5 w-5" />
            </Link>
          </Button>
          <div>
            <h1 className="text-2xl font-bold text-foreground">Editar Licitação</h1>
            <p className="text-sm text-muted-foreground mt-1">
              Atualize os dados da licitação.
            </p>
          </div>
        </div>

        <Card className="border-border">
          <CardHeader>
            <CardTitle>Informações Principais</CardTitle>
            <CardDescription>
              Detalhes essenciais para a identificação da licitação.
            </CardDescription>
          </CardHeader>
          <CardContent>
            {isLoading ? (
              <div className="space-y-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2"><Skeleton className="h-4 w-24" /><Skeleton className="h-10 w-full" /></div>
                  <div className="space-y-2"><Skeleton className="h-4 w-32" /><Skeleton className="h-10 w-full" /></div>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2"><Skeleton className="h-4 w-32" /><Skeleton className="h-10 w-full" /></div>
                  <div className="space-y-2"><Skeleton className="h-4 w-40" /><Skeleton className="h-10 w-full" /></div>
                </div>
                <div className="space-y-2"><Skeleton className="h-4 w-24" /><Skeleton className="h-10 w-full md:w-1/2" /></div>
              </div>
            ) : (
              <form onSubmit={handleSubmit} className="space-y-6">

                <div className="space-y-4">
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="space-y-2">
                      <Label htmlFor="title">Título do Edital <span className="text-destructive">*</span></Label>
                      <Input
                        id="title"
                        name="title"
                        placeholder="Ex: Pregão Eletrônico Nº 01/2026"
                        required
                        value={formData.title}
                        onChange={handleChange}
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="organization">Órgão Responsável <span className="text-destructive">*</span></Label>
                      <Input
                        id="organization"
                        name="organization"
                        placeholder="Ex: Prefeitura Municipal..."
                        required
                        value={formData.organization}
                        onChange={handleChange}
                      />
                    </div>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="space-y-2">
                      <Label htmlFor="publishDate">Data de Publicação</Label>
                      <Input
                        id="publishDate"
                        name="publishDate"
                        type="date"
                        value={formData.publishDate}
                        onChange={handleChange}
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="biddingDate">Data de Abertura (Concorrência)</Label>
                      <Input
                        id="biddingDate"
                        name="biddingDate"
                        type="datetime-local"
                        value={formData.biddingDate}
                        onChange={handleChange}
                      />
                    </div>
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="status">Status Atual</Label>
                    <Select
                      value={formData.status.toString()}
                      onValueChange={handleStatusChange}
                    >
                      <SelectTrigger className="w-full md:w-1/2">
                        <SelectValue placeholder="Selecione o status" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="0">Em Estruturação</SelectItem>
                        <SelectItem value="1">Publicado</SelectItem>
                        <SelectItem value="2">Suspenso</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                </div>

                <div className="flex justify-end pt-4 border-t border-border">
                  <div className="flex gap-2 w-full sm:w-auto">
                    <Button variant="outline" type="button" asChild className="w-full sm:w-auto">
                      <Link href={`/auctions/${auctionId}`}>
                        Cancelar
                      </Link>
                    </Button>
                    <Button type="submit" disabled={isSubmitting} className="w-full sm:w-auto">
                      {isSubmitting ? (
                        <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      ) : (
                        <Save className="mr-2 h-4 w-4" />
                      )}
                      Salvar Alterações
                    </Button>
                  </div>
                </div>

              </form>
            )}
          </CardContent>
        </Card>
      </div>
    </AuthGuard>
  )
}
