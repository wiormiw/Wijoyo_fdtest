import { useAuth } from '../hooks/useAuth';
import { useState } from 'react';
import { changePassword } from '../api/auth';

export default function Profile() {
  const { user, token } = useAuth();
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [success, setSuccess] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChangePassword = async (e: React.FormEvent) => {
    e.preventDefault();
    setSuccess('');
    setError('');
    setLoading(true);
    try {
      await changePassword({ currentPassword, newPassword }, token!);
      setSuccess('Password changed successfully!');
      setCurrentPassword('');
      setNewPassword('');
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to change password');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-xl mx-auto mt-32 bg-white rounded-lg shadow p-6">
      <h1 className="text-2xl font-bold mb-4">Profile</h1>
      <div className="mb-6">
        <div className="mb-2"><span className="font-medium">Username:</span> {user?.userName}</div>
        <div className="mb-2"><span className="font-medium">Email:</span> {user?.email}</div>
        <div className="mb-2 flex items-center gap-2">
          <span className="font-medium">Email Status:</span>
          <span className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-semibold ${user?.emailStatus === 'Verified' ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700'}`}>
            <span className={`w-3 h-3 rounded-full mr-2 ${user?.emailStatus === 'Verified' ? 'bg-green-500' : 'bg-yellow-400 border border-yellow-600'}`}></span>
            {user?.emailStatus}
          </span>
        </div>
      </div>
      <form onSubmit={handleChangePassword} className="space-y-4">
        <h2 className="text-lg font-semibold mb-2">Change Password</h2>
        <div>
          <label className="block text-sm font-medium text-gray-700">Current Password</label>
          <input type="password" className="mt-1 block w-full border rounded px-3 py-2" value={currentPassword} onChange={e => setCurrentPassword(e.target.value)} required />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">New Password</label>
          <input type="password" className="mt-1 block w-full border rounded px-3 py-2" value={newPassword} onChange={e => setNewPassword(e.target.value)} required />
        </div>
        {success && <div className="text-green-600 text-sm">{success}</div>}
        {error && <div className="text-red-600 text-sm">{error}</div>}
        <button type="submit" className="px-4 py-2 bg-indigo-600 text-white rounded hover:bg-indigo-700" disabled={loading}>
          {loading ? 'Changing...' : 'Change Password'}
        </button>
      </form>
    </div>
  );
} 