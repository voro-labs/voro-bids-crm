"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import Link from "next/link"
import { ArrowLeft, Save, Loader2 } from "lucide-react"
import { toast } from "sonner"

import { API_CONFIG, secureApiCall } from "@/lib/api"
import { AuthGuard } from "@/components/auth/auth.guard"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card"

export default function NewAuctionPage() {
  const router = useRouter()
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [formData, setFormData] = useState({
    title: "",
    organization: "",
    biddingDate: "",
    publishDate: "",
    status: 0,
  })

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
      // Formata as datas para envio ISO se fornecidas
      const payload = {
        ...formData,
        biddingDate: formData.biddingDate ? new Date(formData.biddingDate).toISOString() : null,
        publishDate: formData.publishDate ? new Date(formData.publishDate).toISOString() : null,
      }

      const res = await secureApiCall(API_CONFIG.ENDPOINTS.AUCTIONS, {
        method: "POST",
        body: JSON.stringify(payload),
      })

      if (res.hasError) {
        toast.error(res.message || "Erro ao criar licitação.")
      } else {
        toast.success("Licitação criada com sucesso!")
        router.push("/auctions")
        router.refresh()
      }
    } catch (error) {
      toast.error("Erro inesperado.")
      console.error(error)
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <AuthGuard requiredRoles={["Admin", "User", "Legal", "Finance", "Management", "Operational"]}>
      <div className="flex flex-col gap-6 p-6 max-w-3xl mx-auto w-full">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" asChild>
            <Link href="/auctions">
              <ArrowLeft className="h-5 w-5" />
            </Link>
          </Button>
          <div>
            <h1 className="text-2xl font-bold text-foreground">Nova Licitação</h1>
            <p className="text-sm text-muted-foreground mt-1">
              Preencha os dados da nova concorrência ou edital.
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
                  <Label htmlFor="status">Status Inicial</Label>
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
                    <Link href="/auctions">
                      Cancelar
                    </Link>
                  </Button>
                  <Button type="submit" disabled={isSubmitting} className="w-full sm:w-auto">
                    {isSubmitting ? (
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    ) : (
                      <Save className="mr-2 h-4 w-4" />
                    )}
                    Salvar Licitação
                  </Button>
                </div>
              </div>

            </form>
          </CardContent>
        </Card>
      </div>
    </AuthGuard>
  )
}
