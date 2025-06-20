import { useAuth } from '../hooks/useAuth';
import { Link, useNavigate } from 'react-router-dom';
import { useState } from 'react';

export default function Navbar() {
  const { token, user, logout } = useAuth();
  const [mobileOpen, setMobileOpen] = useState(false);
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <nav className="fixed top-4 left-0 right-0 z-50 flex justify-center pointer-events-none">
      <div className="pointer-events-auto w-full max-w-5xl mx-auto bg-white/80 backdrop-blur shadow-lg rounded-2xl px-4 sm:px-8 py-2 flex flex-col md:flex-row items-center justify-between border border-gray-200">
        <div className="flex items-center gap-4 w-full md:w-auto justify-between">
          <Link to="/" className="text-xl font-bold text-indigo-700">BookApp</Link>
          <button className="md:hidden p-2 rounded hover:bg-gray-100" onClick={() => setMobileOpen(o => !o)}>
            <span className="sr-only">Open main menu</span>
            <svg className="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" /></svg>
          </button>
        </div>
        <div className="hidden md:flex gap-6 items-center">
          <Link to="/" className="hover:text-indigo-600 font-medium">Home</Link>
          {token ? (
            <>
              <Link to="/dashboard/books" className="hover:text-indigo-600 font-medium">Book Management</Link>
              <Link to="/dashboard/users" className="hover:text-indigo-600 font-medium">Users</Link>
              <Link to="/profile" className="hover:text-indigo-600 font-medium">Profile</Link>
              <button onClick={handleLogout} className="ml-2 px-3 py-1 bg-red-500 text-white rounded hover:bg-red-600">Logout</button>
            </>
          ) : (
            <>
              <Link to="/login" className="hover:text-indigo-600 font-medium">Login</Link>
              <Link to="/register" className="hover:text-indigo-600 font-medium">Register</Link>
            </>
          )}
        </div>
      </div>
      {/* Mobile menu */}
      {mobileOpen && (
        <div className="fixed top-20 left-0 right-0 z-50 flex justify-center">
          <div className="w-full max-w-5xl mx-auto bg-white/90 backdrop-blur shadow-lg rounded-b-2xl px-4 py-4 flex flex-col gap-2 border border-t-0 border-gray-200">
            <Link to="/" className="py-2 hover:text-indigo-600 font-medium" onClick={() => setMobileOpen(false)}>Home</Link>
            {token ? (
              <>
                <Link to="/dashboard/books" className="py-2 hover:text-indigo-600 font-medium" onClick={() => setMobileOpen(false)}>Book Management</Link>
                <Link to="/dashboard/users" className="py-2 hover:text-indigo-600 font-medium" onClick={() => setMobileOpen(false)}>Users</Link>
                <Link to="/profile" className="py-2 hover:text-indigo-600 font-medium" onClick={() => setMobileOpen(false)}>Profile</Link>
                <button onClick={() => { setMobileOpen(false); handleLogout(); }} className="py-2 text-left text-red-500 font-medium">Logout</button>
              </>
            ) : (
              <>
                <Link to="/login" className="py-2 hover:text-indigo-600 font-medium" onClick={() => setMobileOpen(false)}>Login</Link>
                <Link to="/register" className="py-2 hover:text-indigo-600 font-medium" onClick={() => setMobileOpen(false)}>Register</Link>
              </>
            )}
          </div>
        </div>
      )}
    </nav>
  );
} 