import { useEffect, useState } from "react";
import { useIsMobile } from "./use-mobile";

export function useHideOnScroll() {
  const [isHeaderHidden, setIsHeaderHidden] = useState(false);
  const isMobile = useIsMobile();

  useEffect(() => {
    if (!isMobile) return;

    let ticking = false;

    let lastY = window.scrollY;
    const threshold = 10;
    const offset = 20;
    const onScroll = () => {
      if (ticking) return;
      ticking = true;

      requestAnimationFrame(() => {
        const currentY = window.scrollY;
        if (currentY < offset) setIsHeaderHidden(false);
        else if (currentY - lastY > threshold) setIsHeaderHidden(true);
        else if (currentY - lastY < -threshold) setIsHeaderHidden(false);
        lastY = currentY;

        ticking = false;
      });
    };
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => {
      window.removeEventListener("scroll", onScroll);
    };
  }, [isMobile]);

  return isHeaderHidden && isMobile;
}
