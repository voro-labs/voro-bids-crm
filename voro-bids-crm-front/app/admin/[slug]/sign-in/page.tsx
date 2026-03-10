"use client"

import type React from "react"
import { useState, useEffect } from "react"
import { useRouter, useParams } from "next/navigation"
import { Eye, EyeOff, AlertCircle, Loader2 } from "lucide-react"
import { useAuth } from "@/contexts/auth.context"
import { useSignIn } from "@/hooks/use-sign-in.hook"
import { SignInDto } from "@/types/DTOs/sign-in.interface"
import { LoadingSimple } from "@/components/ui/custom/loading/loading-simple"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { useSlugTenant } from "@/contexts/slug-tenant.context"
import { TenantBrandHeader } from "../brand-header"

export default function SlugSignInPage() {
  const router = useRouter()
  const params = useParams<{ slug: string }>()
  const slug = params.slug
  const tenant = useSlugTenant()

  const { user, loading: authLoading } = useAuth()
  const { signIn, loading, error, clearError } = useSignIn()

  const [formData, setFormData] = useState<SignInDto>({ email: "", password: "" })
  const [showPassword, setShowPassword] = useState(false)
  const [fieldErrors, setFieldErrors] = useState({ email: "", password: "" })

  useEffect(() => {
    if (!authLoading && user?.token) {
      router.replace("/")
    }
  }, [user, authLoading, router])

  const validateForm = (): boolean => {
    const errors: any = { email: "", password: "" }
    if (!formData.email) errors.email = "Email é obrigatório"
    else if (!/\S+@\S+\.\S+/.test(formData.email)) errors.email = "Email inválido"
    if (!formData.password) errors.password = "Senha é obrigatória"
    else if (formData.password.length < 6) errors.password = "Senha deve ter pelo menos 6 caracteres"
    setFieldErrors(errors)
    return !errors.email && !errors.password
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    clearError()
    if (!validateForm()) return
    const result = await signIn(formData)
    if (result.success) router.push("/")
  }

  const handleInputChange = (field: keyof SignInDto, value: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }))
    if (fieldErrors[field]) setFieldErrors((prev) => ({ ...prev, [field]: "" }))
  }

  if (authLoading || user?.token) return <LoadingSimple />

  return (
    <div className="flex min-h-screen items-center justify-center bg-background p-4">
      <div className="w-full max-w-sm rounded-2xl border border-border bg-card p-8 shadow-lg">
        <TenantBrandHeader tenant={tenant} />

        {error && (
          <div className="mb-5 flex items-start gap-2 rounded-lg border border-destructive/30 bg-destructive/10 p-3 text-destructive">
            <AlertCircle className="mt-0.5 h-4 w-4 shrink-0" />
            <p className="text-sm">{error}</p>
          </div>
        )}

        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <div className="flex flex-col gap-1.5">
            <Label htmlFor="email">Email</Label>
            <Input
              type="email"
              id="email"
              value={formData.email}
              onChange={(e) => handleInputChange("email", e.target.value)}
              className={fieldErrors.email ? "border-destructive" : ""}
              placeholder="seu@email.com"
              autoComplete="email"
              disabled={loading}
            />
            {fieldErrors.email && (
              <span className="text-xs text-destructive">{fieldErrors.email}</span>
            )}
          </div>

          <div className="flex flex-col gap-1.5">
            <Label htmlFor="password">Senha</Label>
            <div className="relative">
              <Input
                type={showPassword ? "text" : "password"}
                id="password"
                value={formData.password}
                onChange={(e) => handleInputChange("password", e.target.value)}
                className={`pr-10 ${fieldErrors.password ? "border-destructive" : ""}`}
                placeholder="••••••••"
                autoComplete="current-password"
                disabled={loading}
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute inset-y-0 right-0 flex items-center pr-3 text-muted-foreground hover:text-foreground"
                disabled={loading}
              >
                {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
              </button>
            </div>
            {fieldErrors.password && (
              <span className="text-xs text-destructive">{fieldErrors.password}</span>
            )}
          </div>

          <div className="flex justify-end">
            <a
              href={`/admin/${slug}/forgot-password`}
              className="text-sm text-primary hover:underline"
            >
              Esqueceu a senha?
            </a>
          </div>

          <Button type="submit" disabled={loading} className="w-full">
            {loading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {loading ? "Entrando..." : "Entrar"}
          </Button>
        </form>
      </div>
    </div>
  )
}
