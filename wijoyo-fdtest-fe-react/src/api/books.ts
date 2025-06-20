import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
});

export interface Book {
  id: string;
  title: string;
  author: string;
  description: string;
  coverUrl: string;
  rating: number;
  userName?: string;
  dateUploaded: string;
}

export interface BookListResponse {
  items: Book[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export async function getPublicBooks(params: any): Promise<BookListResponse> {
  const { data } = await api.get('/Public/Books', { params });
  return data;
}

export async function getUserBooks(params: any, token: string): Promise<BookListResponse> {
  const { data } = await api.get('/Books', {
    params,
    headers: { Authorization: `Bearer ${token}` },
  });
  return data;
}

export async function createBook(payload: any, token: string): Promise<Book> {
  const { data } = await api.post('/Books', payload, {
    headers: { Authorization: `Bearer ${token}` },
  });
  return data;
}

export async function updateBook(id: string, payload: any, token: string): Promise<Book> {
  const { data } = await api.put(`/Books/${id}`, { ...payload, bookId: id }, {
    headers: { Authorization: `Bearer ${token}` },
  });
  return data;
}

export async function deleteBook(id: string, token: string): Promise<void> {
  await api.delete(`/Books/${id}`, {
    data: { bookId: id },
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}

export async function uploadBookCover(file: File, token: string): Promise<string> {
  const formData = new FormData();
  formData.append('file', file);
  const { data } = await api.post('/Books/cover/upload', formData, {
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'multipart/form-data',
    },
  });
  return data.url;
}

export async function getBookDetail(id: string, token: string): Promise<Book> {
  const { data } = await api.get(`/Books/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  return data;
} 