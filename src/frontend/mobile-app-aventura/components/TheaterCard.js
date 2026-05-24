import { StatusBar } from 'expo-status-bar';
import { View, Text, StyleSheet } from 'react-native';

import TheaterCard from './components/TheaterCard';

import { colors } from './constants/colors';
import { typography } from './constants/typography';
import { spacing } from './constants/spacing';

export default function App() {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>Teste TheaterCard</Text>

      <TheaterCard
        name="Teatro Renault"
        location="São Paulo, SP"
        image={{ uri: 'https://picsum.photos/400/250?random=10' }}
        onPress={() => console.log('Teatro Renault')}
      />

      <StatusBar style="auto" />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.white,
    paddingHorizontal: spacing.xl,
    justifyContent: 'center',
  },

  title: {
    fontSize: typography.size.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.purplePrimary,
    marginBottom: spacing.xl,
  },
});