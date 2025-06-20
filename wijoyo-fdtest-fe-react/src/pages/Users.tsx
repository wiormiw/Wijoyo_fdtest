import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { getUsers } from '../api/users';
import { useAuth } from '../hooks/useAuth';

const PAGE_SIZE = 10;

export default function Users() {
  const { token } = useAuth();
  const [page, setPage] = useState(1);
  const [emailStatus, setEmailStatus] = useState('');
  const [search, setSearch] = useState('');

  const { data, isLoading, isError } = useQuery({
    queryKey: ['users', { page, emailStatus, search, pageSize: PAGE_SIZE, token }],
    queryFn: () => getUsers({
      PageNumber: page,
      PageSize: PAGE_SIZE,
      EmailStatus: emailStatus || undefined,
      Search: search || undefined,
    }, token!),
    enabled: !!token,
  });

  return (
    <div className="max-w-7xl mx-auto py-8 px-2">
      <div className="flex flex-col sm:flex-row justify-between items-center mb-6 gap-4">
        <h2 className="text-2xl font-bold">User List</h2>
      </div>
      <form
        className="flex flex-col sm:flex-row gap-4 mb-6 justify-center"
        onSubmit={e => {
          e.preventDefault();
          setPage(1);
        }}
      >
        <select
          className="px-4 py-2 border rounded-md w-full sm:w-48 focus:outline-none focus:ring-2 focus:ring-indigo-500"
          value={emailStatus}
          onChange={e => setEmailStatus(e.target.value)}
        >
          <option value="">All Statuses</option>
          <option value="Verified">Verified</option>
          <option value="Unverified">Unverified</option>
        </select>
        <input
          type="text"
          placeholder="Search by name or email..."
          className="px-4 py-2 border rounded-md w-full sm:w-64 focus:outline-none focus:ring-2 focus:ring-indigo-500"
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
        <button
          type="submit"
          className="px-6 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 transition"
        >
          Filter
        </button>
      </form>
      <div className="bg-white shadow-lg rounded-2xl overflow-x-auto">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-100 rounded-t-2xl">
            <tr>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Username</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email Status</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {isLoading ? (
              <tr>
                <td colSpan={3} className="text-center py-8 text-gray-400">Loading...</td>
              </tr>
            ) : isError ? (
              <tr>
                <td colSpan={3} className="text-center py-8 text-red-500">Failed to load users.</td>
              </tr>
            ) : data && data.items.length === 0 ? (
              <tr>
                <td colSpan={3} className="text-center py-8 text-gray-400">No users found.</td>
              </tr>
            ) : (
              data?.items.map(user => (
                <tr key={user.email} className="hover:bg-gray-50 transition">
                  <td className="px-4 py-3 font-semibold text-gray-900">{user.userName}</td>
                  <td className="px-4 py-3 text-gray-700">{user.email}</td>
                  <td className="px-4 py-3 text-gray-500">{user.emailStatus}</td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
      {/* Pagination */}
      {data && (data.totalPages > 1) && (
        <div className="flex justify-center items-center gap-2 mt-6">
          <button
            className="px-3 py-1 rounded bg-gray-200 hover:bg-gray-300 disabled:opacity-50"
            onClick={() => setPage(p => Math.max(1, p - 1))}
            disabled={page === 1}
          >
            Previous
          </button>
          <span className="text-gray-700 font-medium">
            Page {page} of {data.totalPages}
          </span>
          <button
            className="px-3 py-1 rounded bg-gray-200 hover:bg-gray-300 disabled:opacity-50"
            onClick={() => setPage(p => Math.min(data.totalPages, p + 1))}
            disabled={page === data.totalPages}
          >
            Next
          </button>
        </div>
      )}
    </div>
  );
} 