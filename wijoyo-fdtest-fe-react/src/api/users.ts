import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
});

export interface UserListItem {
  userName: string;
  email: string;
  emailStatus: string;
}

export interface UserListResponse {
  items: UserListItem[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export async function getUsers(params: any, token: string): Promise<UserListResponse> {
  const { data } = await api.get('/Users/filters', {
    params,
    headers: { Authorization: `Bearer ${token}` },
  });
  return data;
} 