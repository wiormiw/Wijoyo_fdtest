import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export default function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [emailError, setEmailError] = useState('');
  const [passwordError, setPasswordError] = useState('');
  const navigate = useNavigate();
  const auth = useAuth();

  const validate = () => {
    setEmailError('');
    setPasswordError('');
    let valid = true;
    if (!email.match(/^[^@\s]+@[^@\s]+$/)) {
      setEmailError('Invalid email address');
      valid = false;
    }
    if (!password) {
      setPasswordError('Password is required');
      valid = false;
    }
    return valid;
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    try {
      await auth.login({ email, password });
      navigate('/dashboard/books');
    } catch (e) {
      // error handled by auth.loginError
    }
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-50 px-2">
      <div className="w-full max-w-md bg-white/90 rounded-2xl shadow-xl p-8 mt-20">
        <h1 className="text-2xl font-bold mb-6 text-center">Sign in to your account</h1>
        <form onSubmit={onSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700">Email</label>
            <input
              type="email"
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              value={email}
              onChange={e => setEmail(e.target.value)}
              required
            />
            {emailError && <p className="mt-1 text-sm text-red-600">{emailError}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700">Password</label>
            <input
              type="password"
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              value={password}
              onChange={e => setPassword(e.target.value)}
              required
            />
            {passwordError && <p className="mt-1 text-sm text-red-600">{passwordError}</p>}
          </div>
          {auth.loginError && <p className="text-sm text-red-600">{String(auth.loginError)}</p>}
          <button
            type="submit"
            disabled={auth.loginStatus === 'pending'}
            className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
          >
            {auth.loginStatus === 'pending' ? 'Signing in...' : 'Sign in'}
          </button>
        </form>
        <div className="mt-6 text-center">
          <a href="/register" className="font-medium text-indigo-600 hover:text-indigo-500">
            Create a new account
          </a>
          <div className="mt-2">
            <a href="/forgot-password" className="font-medium text-indigo-600 hover:text-indigo-500">
              Forgot password?
            </a>
          </div>
        </div>
      </div>
    </div>
  );
} 