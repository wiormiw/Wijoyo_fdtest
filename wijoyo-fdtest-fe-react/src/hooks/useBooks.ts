import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getPublicBooks, getUserBooks, createBook, updateBook as updateBookApi, deleteBook, uploadBookCover } from '../api/books';
import type { BookListResponse } from '../api/books';
import { useAuth } from './useAuth';

export function usePublicBooks(params: Record<string, any>) {
  return useQuery<BookListResponse>({
    queryKey: ['publicBooks', params],
    queryFn: () => getPublicBooks(params),
  });
}

export function useUserBooks(params: Record<string, any>) {
  const { token } = useAuth();
  return useQuery<BookListResponse>({
    queryKey: ['userBooks', params, token],
    queryFn: () => token ? getUserBooks(params, token) : Promise.reject('No token'),
    enabled: !!token,
  });
}

export function useCreateBook() {
  const { token } = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: any) => token ? createBook(payload, token) : Promise.reject('No token'),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['userBooks'] });
    },
  });
}

export function useUpdateBook() {
  const { token } = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: any }) =>
      token ? updateBookApi(id, { ...payload, BookId: id }, token) : Promise.reject('No token'),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['userBooks'] });
    },
  });
}

export function useDeleteBook() {
  const { token } = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => token ? deleteBook(id, token) : Promise.reject('No token'),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['userBooks'] });
    },
  });
}

export function useUploadBookCover() {
  const { token } = useAuth();
  return useMutation({
    mutationFn: (file: File) => token ? uploadBookCover(file, token) : Promise.reject('No token'),
  });
} 