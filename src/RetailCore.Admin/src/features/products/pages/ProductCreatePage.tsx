import { useNavigate } from "react-router-dom";

import { ProductSetupHeader } from "../components/product-setup/ProductSetupHeader";
import {
  ProductSetupProgress,
  STEPS,
} from "../components/product-setup/ProductSetupProgress";
import { Step1BasicInfo } from "../components/product-setup/steps/Step1BasicInfo";

export default function ProductCreatePage() {
  const navigate = useNavigate();

  const step = STEPS[0];

  return (
    <div className="mx-auto max-w-7xl space-y-6 p-4 md:p-6">
      {/* Header */}
      <ProductSetupHeader
        title="Create Product"
        subtitle={`Step ${step.number} of ${STEPS.length} · ${step.title}`}
        onBack={() => navigate("/admin/products")}
      />

      {/* Progress */}
      <ProductSetupProgress currentStep={step.number} />

      {/* Step Content */}
      <div>
        <Step1BasicInfo
          onCompleted={(createdProduct) => {
            navigate(
              `/admin/products/${createdProduct.id}/setup?step=${STEPS[1].number}`,
              {
                replace: true,
              },
            );
          }}
          onCancel={() => navigate("/admin/products")}
        />
      </div>
    </div>
  );
}
