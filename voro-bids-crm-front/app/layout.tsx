import type { Metadata } from 'next'
import { Geist } from 'next/font/google'
import { Analytics } from '@vercel/analytics/next'
import './globals.css'
import { AuthProvider } from '@/contexts/auth.context'
import { ThemeProvider } from '@/components/theme-provider'
import { SpeedInsights } from "@vercel/speed-insights/next"
import { Main } from "@/components/layout/admin/main"
import { TenantThemeProvider } from "@/contexts/tenant-theme.context"

const _geist = Geist({ subsets: ["latin"] });

const DEFAULT_METADATA: Metadata = {
  title: 'Bids CRM - Gestão de Licitações e Leilões',
  description: 'Sistema para gestão de licitações e leilões, com controle de clientes, acompanhamento de editais, propostas e automação do processo de participação em licitações.',
  keywords: [
    "sistema para gestão de licitações",
    "CRM para licitações",
    "software para gestão de licitações",
    "sistema para leilões",
    "gestão de propostas e lances",
    "controle de editais de licitação",
    "gerenciamento de clientes para licitações",
    "automação de processos de licitação",
    "acompanhamento de licitações",
    "sistema para empresas que participam de licitações",
    "controle de leilões e arremates",
    "gestão de oportunidades de licitação",
    "software para acompanhamento de editais",
    "CRM para empresas de leilão",
    "plataforma de gestão de licitações"
  ],
  generator: "vorolabs.app",

}

export async function generateMetadata(): Promise<Metadata> {
  try {
    const { cookies } = await import("next/headers")
    const cookieStore = await cookies()
    const slug = cookieStore.get("voro_tenant_slug")?.value

    if (!slug) return DEFAULT_METADATA

    const baseUrl = `${process.env.NEXT_PUBLIC_BASE_URL}/${process.env.NEXT_PUBLIC_API_URL}`
    const res = await fetch(`${baseUrl}/tenant/slug/${encodeURIComponent(slug)}`, {
      cache: "no-store", // always fresh — never stale cached metadata
    })

    if (!res.ok) return DEFAULT_METADATA

    const json = await res.json()
    const tenant = json?.data

    if (!tenant?.name) return DEFAULT_METADATA

    return {
      ...DEFAULT_METADATA,
      title: `${tenant.name} - CRM`,
      icons: tenant.logoUrl
        ? {
          icon: tenant.logoUrl,
          shortcut: tenant.logoUrl,
          apple: tenant.logoUrl,
        }
        : DEFAULT_METADATA.icons,
    }
  } catch {
    return DEFAULT_METADATA
  }
}


export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode
}>) {
  return (
    <html lang="pt-BR" className=" scroll-smooth" suppressHydrationWarning>
      <body className={`${_geist.className} font-sans antialiased`}>
        <ThemeProvider
          attribute="class"
          defaultTheme="system"
          enableSystem
          disableTransitionOnChange
        >
          <TenantThemeProvider>
            <AuthProvider>
              <Main>
                {children}
              </Main>
            </AuthProvider>
          </TenantThemeProvider>
        </ThemeProvider>
        {process.env.NODE_ENV === 'production' && (
          <>
            <Analytics />
            <SpeedInsights />
          </>
        )}
      </body>
    </html>
  )
}
