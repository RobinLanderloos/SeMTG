// src/api.ts

const BASE_URL = import.meta.env.VITE_API_BASE_URL;

export interface Hit {
  card: CardResult;
  score: number;
}

export interface CardResult {
  id: string; // Guid as string
  name: string;
  vectors?: number[]; // float[] in C#
  editions: CardEditionResult[];
}

export interface CardEditionResult {
  id: string; // Guid as string
  name: string;
  releasedAt: string; // DateOnly as ISO date string
  scryfallUri: string;
  manaCost: string;
  typeLine: string;
  oracleText: string;
  imageUri: string;
}

export async function searchCards(
  query: string,
  limit: number = 20
): Promise<Hit[]> {
  const url = new URL("/search", BASE_URL);
  url.searchParams.append("query", query);
  url.searchParams.append("limit", limit.toString());

  const response = await fetch(url.toString());

  if (!response.ok) {
    throw new Error(`Search request failed: ${response.statusText}`);
  }

  const data: Hit[] = await response.json();
  console.log(data);
  return data;
}
