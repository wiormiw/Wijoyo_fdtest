import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import * as api from '../api/auth';

export function useAuth() {
  const queryClient = useQueryClient();
  const [token, setToken] = useState(() => localStorage.getItem('accessToken'));

  // Login mutation
  const loginMutation = useMutation({
    mutationFn: api.login,
    onSuccess: (data) => {
      localStorage.setItem('accessToken', data.accessToken);
      setToken(data.accessToken);
      queryClient.invalidateQueries({ queryKey: ['profile'] });
    },
  });

  // Register mutation
  const registerMutation = useMutation({
    mutationFn: api.register,
    onSuccess: (data) => {
      localStorage.setItem('accessToken', data.accessToken);
      setToken(data.accessToken);
      queryClient.invalidateQueries({ queryKey: ['profile'] });
    },
  });

  // Profile query
  const profileQuery = useQuery({
    queryKey: ['profile', token],
    queryFn: () => (token ? api.getProfile(token) : Promise.reject('No token')),
    enabled: !!token,
  });

  function logout() {
    localStorage.removeItem('accessToken');
    setToken(null);
    queryClient.removeQueries({ queryKey: ['profile'] });
  }

  return {
    token,
    login: loginMutation.mutateAsync,
    loginStatus: loginMutation.status,
    loginError: loginMutation.error && (loginMutation.error instanceof Error && (loginMutation.error as any).response?.status === 401)
      ? 'Username or password may be wrong or unregistered.'
      : loginMutation.error,
    register: registerMutation.mutateAsync,
    registerStatus: registerMutation.status,
    registerError: registerMutation.error,
    user: profileQuery.data,
    userStatus: profileQuery.status,
    userError: profileQuery.error,
    logout,
  };
} 