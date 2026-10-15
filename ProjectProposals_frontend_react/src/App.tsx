import AppRouter from "@/routes";
import { LanguageProvider } from "@/i18n";

export default function App() {
  return (
    <LanguageProvider>
      <AppRouter />
    </LanguageProvider>
  );
}
