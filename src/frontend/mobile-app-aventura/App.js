import { StatusBar } from 'expo-status-bar';
import { Text, View, StyleSheet } from 'react-native';
import ScreenContainer from './screens/ScreenContainer';
import { colors } from './constants/colors';
import { typography } from './constants/typography';
import { spacing } from './constants/spacing';

export default function App() {
  return (
    <ScreenContainer>
      <View style={styles.content}>
        <Text style={styles.title}>Portal Aventura</Text>
        <Text style={styles.subtitle}>
          Base visual do frontend configurada com sucesso.
        </Text>
      </View>

      <StatusBar style="auto" />
    </ScreenContainer>
  );
}

const styles = StyleSheet.create({
  content: {
    flex: 1,
    justifyContent: 'center',
    gap: spacing.sm,
  },
  title: {
    fontSize: typography.size.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.purplePrimary,
  },
  subtitle: {
    fontSize: typography.size.md,
    color: colors.grayText,
  },
});
