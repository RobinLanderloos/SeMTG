// src/api.ts

const BASE_URL = window.__ENV__?.VITE_API_BASE_URL;

export interface SearchResult {
  cards: SearchResultCard[];
}

export interface SearchResultCard {
  id: string;
  name: string;
  score: number;
  imageUris: ImageUris;
}

export interface ImageUris {
  small: string;
  normal: string;
  large: string;
  png: string;
  artCrop: string;
  borderCrop: string;
}

export interface CardDetailResult {
  id: string;
  name: string;
  editions: CardEditionDetail[];
}

export interface CardEditionDetail {
  id: string;
  name: string;
  typeLine: string;
  oracleText: string;
  imageUris: ImageUris;
}

export async function searchCards(
  query: string,
  limit: number = 20
): Promise<SearchResult> {
  const url = new URL("/search", BASE_URL);
  url.searchParams.append("query", query);
  url.searchParams.append("limit", limit.toString());

  const response = await fetch(url.toString());

  if (!response.ok) {
    throw new Error(`Search request failed: ${response.statusText}`);
  }

  const data: SearchResult = await response.json();
  return data;
}

export async function getCard(id: string): Promise<CardDetailResult> {
  const url = new URL(`/card/${id}`, BASE_URL);

  const response = await fetch(url.toString());

  if (!response.ok) {
    throw new Error(`Card request failed: ${response.statusText}`);
  }

  const data: CardDetailResult = await response.json();
  return data;
}
