import { StatusBar } from 'expo-status-bar';
import AuthScreen from './screens/AuthScreen';
import HomeScreen from './screens/HomeScreen';

export default function App() {
  return (
    <>
      <AuthScreen />
      <StatusBar style="auto" />
    </>
  );
}