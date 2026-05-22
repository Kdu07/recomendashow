import { StatusBar } from 'expo-status-bar';
import ComponentsPreviewScreen from './screens/ComponentsPreviewScreen';

export default function App() {
  return (
    <>
      <ComponentsPreviewScreen />
      <StatusBar style="auto" />
    </>
  );
}