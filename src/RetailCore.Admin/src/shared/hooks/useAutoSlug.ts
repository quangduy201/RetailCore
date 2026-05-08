import { useEffect, useRef, useState } from "react";
import { generateSlug, sanitizeSlug } from "@/shared/utils/slugUtils";

type UseAutoSlugOptions = {
  name: string;
  onSlugChange: (value: string) => void;
};

export function useAutoSlug({ name, onSlugChange }: UseAutoSlugOptions) {
  const [auto, setAuto] = useState(true);

  const manualSlugRef = useRef<string>("");

  const generated = generateSlug(name || "");

  useEffect(() => {
    if (!auto) return;

    onSlugChange(generated);
  }, [generated, auto, onSlugChange]);

  const handleSlugChange = (value: string) => {
    const cleaned = sanitizeSlug(value);

    onSlugChange(cleaned);

    if (auto) {
      setAuto(false);
      manualSlugRef.current = cleaned;
    } else {
      manualSlugRef.current = cleaned;
    }
  };

  const enableAuto = () => {
    const next = generateSlug(name || "");
    setAuto(true);

    manualSlugRef.current = "";
    onSlugChange(next);
  };

  const disableAuto = () => {
    setAuto(false);

    if (manualSlugRef.current) {
      onSlugChange(manualSlugRef.current);
    }
  };

  return {
    auto,
    setAuto,
    handleSlugChange,
    enableAuto,
    disableAuto,
    generatedSlug: generated,
  };
}
