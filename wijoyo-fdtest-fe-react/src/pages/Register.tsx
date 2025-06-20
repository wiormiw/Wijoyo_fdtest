import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export default function Register() {
  const [userName, setUserName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [userNameError, setUserNameError] = useState('');
  const [emailError, setEmailError] = useState('');
  const [passwordError, setPasswordError] = useState('');
  const navigate = useNavigate();
  const auth = useAuth();

  const validate = () => {
    setUserNameError('');
    setEmailError('');
    setPasswordError('');
    let valid = true;
    if (!userName) {
      setUserNameError('Username is required');
      valid = false;
    }
    if (!email.match(/^[^@\s]+@[^@\s]+$/)) {
      setEmailError('Invalid email address');
      valid = false;
    }
    if (!password || password.length < 6) {
      setPasswordError('Password must be at least 6 characters');
      valid = false;
    }
    return valid;
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    try {
      await auth.register({ email, password, userName });
      navigate('/dashboard/books');
    } catch (e) {
      // error handled by auth.registerError
    }
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-50 px-2">
      <div className="w-full max-w-md bg-white/90 rounded-2xl shadow-xl p-8 mt-20">
        <h1 className="text-2xl font-bold mb-6 text-center">Create a new account</h1>
        <form onSubmit={onSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700">Username</label>
            <input
              className="mt-1 block w-full border rounded px-3 py-2"
              value={userName}
              onChange={e => setUserName(e.target.value)}
              required
            />
            {userNameError && <p className="mt-1 text-sm text-red-600">{userNameError}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700">Email</label>
            <input
              type="email"
              className="mt-1 block w-full border rounded px-3 py-2"
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
              className="mt-1 block w-full border rounded px-3 py-2"
              value={password}
              onChange={e => setPassword(e.target.value)}
              required
            />
            {passwordError && <p className="mt-1 text-sm text-red-600">{passwordError}</p>}
          </div>
          {auth.registerError && <p className="text-sm text-red-600">{String(auth.registerError)}</p>}
          <button
            type="submit"
            disabled={auth.registerStatus === 'pending'}
            className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
          >
            {auth.registerStatus === 'pending' ? 'Registering...' : 'Register'}
          </button>
        </form>
        <div className="mt-6 text-center">
          <a href="/login" className="font-medium text-indigo-600 hover:text-indigo-500">
            Sign in to an existing account
          </a>
        </div>
      </div>
    </div>
  );
} 