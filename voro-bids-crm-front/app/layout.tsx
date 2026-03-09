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

export const metadata: Metadata = {
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
  icons: {
    icon: [
      {
        url: "/icon-light-32x32.png",
        media: "(prefers-color-scheme: light)",
      },
      {
        url: "/icon-dark-32x32.png",
        media: "(prefers-color-scheme: dark)",
      },
      {
        url: "/icon.png",
        type: "image/png",
      },
    ],
    apple: "/apple-icon.png",
  },
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
