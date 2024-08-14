import { useEffect, useState } from "https://esm.sh/react@18.2.0";

export const useResizeHandled = () => {
  const [resizeHandled, setResizeHandled] = useState(false);

  useEffect(() => {
    globalThis.addEventListener("resize", () => {
      setResizeHandled(false);
    });
  }, []);

  return [resizeHandled, setResizeHandled] as const;
};
