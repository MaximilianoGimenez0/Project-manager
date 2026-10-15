import { Outlet } from "react-router-dom";
import { Navbar, Footer, AlertContainer } from "@/components";
import styles from "./MainLayout.module.css";

export default function MainLayout() {
  return (
    <div className={styles.layout}>
      <AlertContainer />
      <Navbar />
      <main className={styles.content}>
        <Outlet />
      </main>
      <Footer />
    </div>
  );
}
