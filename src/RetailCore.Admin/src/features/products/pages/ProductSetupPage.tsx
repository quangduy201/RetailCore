import { useEffect, useState } from "react";
import { useNavigate, useParams, useSearchParams } from "react-router-dom";
import { toast } from "sonner";

import type { UUID } from "@/shared/types/common";

import { productApi } from "../api/product.api";
import { ProductSetupHeader } from "../components/product-setup/ProductSetupHeader";
import {
  ProductSetupProgress,
  STEPS,
} from "../components/product-setup/ProductSetupProgress";
import { Step1BasicInfo } from "../components/product-setup/steps/Step1BasicInfo";
import { Step2Variants } from "../components/product-setup/steps/Step2Variants";
import { Step3Images } from "../components/product-setup/steps/Step3Images";
import type { ProductDetailDto } from "../types/types";

export default function ProductSetupPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const [searchParams, setSearchParams] = useSearchParams();

  const productId = id as UUID;
  const [product, setProduct] = useState<ProductDetailDto | null>(null);

  const initialStep = Number(searchParams.get("step") || "1");

  const [currentStep, setCurrentStep] = useState(
    Number.isNaN(initialStep) || initialStep < 1 || initialStep > STEPS.length
      ? STEPS[0]
      : STEPS[initialStep - 1],
  );

  useEffect(() => {
    const initializeWizard = async () => {
      try {
        if (productId) {
          const productData = await productApi.getById(productId);

          console.log(productData);
          setProduct(productData);
        }
      } catch (error) {
        console.error(error);
        toast.error("Failed to initialize product setup");

        navigate("/products");
      }
    };

    void initializeWizard();
  }, []);

  const updateStepInUrl = (step: number) => {
    const params = new URLSearchParams(searchParams);

    params.set("step", step.toString());

    setSearchParams(params);
  };

  const goToStep = (step: number) => {
    setCurrentStep(STEPS[step - 1]);
    updateStepInUrl(step);
  };

  return (
    <div className="mx-auto max-w-7xl space-y-6 p-4 md:p-6">
      <ProductSetupHeader
        title="Setup Product"
        subtitle={`Step ${currentStep.number} of ${STEPS.length} · ${currentStep.title}`}
        onBack={() => navigate(`/products/${productId}`)}
      />

      <ProductSetupProgress currentStep={currentStep.number} />

      {/* Step Content */}
      <div>
        {currentStep.number === 1 && (
          <Step1BasicInfo
            product={product}
            onCompleted={(createdProduct) => {
              setProduct(createdProduct);
              goToStep(2);
            }}
            onCancel={() => {
              if (product === null) {
                navigate("/products");
              } else {
                navigate(`/products/${product.id}`);
              }
            }}
          />
        )}

        {currentStep.number === 2 && (
          <Step2Variants
            product={product}
            onBack={() => goToStep(1)}
            onCompleted={(updatedProduct) => {
              setProduct(updatedProduct);
              goToStep(3);
            }}
          />
        )}

        {currentStep.number === 3 && (
          <Step3Images
            product={product}
            onBack={() => goToStep(2)}
            onCompleted={() => navigate(`/products/${product?.id}`)}
          />
        )}
      </div>
    </div>
  );
}
