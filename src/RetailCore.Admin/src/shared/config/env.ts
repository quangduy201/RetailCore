const getEnv = (key: string): string => {
  const value = import.meta.env[key]?.trim();

  if (!value) {
    throw new Error(`Missing environment variable: ${key}`);
  }

  return value;
};

export const env = {
  apiUrl: getEnv("VITE_API_URL"),
};
