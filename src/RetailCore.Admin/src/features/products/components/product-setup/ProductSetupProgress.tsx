import { Card } from "@heroui/react";
import { Check } from "lucide-react";

export interface Step {
  number: number;
  title: string;
}

interface Props {
  currentStep: number;
}

export const STEPS: Step[] = [
  {
    number: 1,
    title: "Basic Info & Attributes",
  },
  {
    number: 2,
    title: "Generate Variants",
  },
  {
    number: 3,
    title: "Variant Images",
  },
];

export function ProductSetupProgress({ currentStep }: Readonly<Props>) {
  return (
    <Card className="overflow-hidden border border-default-200 p-8">
      <div className="flex items-center justify-evenly">
        {STEPS.map((step, index) => {
          const isCompleted = step.number < currentStep;

          const isActive = step.number === currentStep;

          const isLast = index === STEPS.length - 1;

          return (
            <div
              key={step.number}
              className={`flex items-center ${!isLast ? "flex-1" : ""}`}
            >
              <div className="flex flex-col items-center">
                <div
                  className={`flex h-14 w-14 items-center justify-center rounded-full border-2 text-sm font-bold transition-all duration-200 ${
                    isCompleted
                      ? "border-success bg-success text-white"
                      : isActive
                        ? "border-accent bg-accent text-white shadow-lg shadow-accent/30"
                        : "border-dashed border-default-300 bg-background text-default-500"
                  }`}
                >
                  {isCompleted ? <Check className="size-5" /> : step.number}
                </div>

                <div className="mt-3 text-center">
                  <p
                    className={`text-sm font-semibold ${
                      isActive
                        ? "text-accent"
                        : isCompleted
                          ? "text-success"
                          : "text-default-500"
                    }`}
                  >
                    {step.title}
                  </p>
                </div>
              </div>

              {!isLast && (
                <div className="mb-10 flex-1">
                  <div
                    className={`h-0.5 w-full ${
                      step.number < currentStep
                        ? "bg-success"
                        : "border-t-2 border-dashed border-default-300"
                    }`}
                  />
                </div>
              )}
            </div>
          );
        })}
      </div>
    </Card>
  );
}
