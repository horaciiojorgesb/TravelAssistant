import type { Metadata } from "next";
import "./globals.css";
import { UserProvider } from '@auth0/nextjs-auth0/client';

export const metadata =  {
  title: "Travel Assistant",
  description: "Vodacom Dev",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <UserProvider>
    <html lang="en">
      <body>{children}</body>
    </html>
    </UserProvider>
  )
}

