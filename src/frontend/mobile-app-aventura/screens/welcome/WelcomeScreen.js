import React from 'react';
import { View, Text, Image, StyleSheet, SafeAreaView } from 'react-native';

import { useWelcomeViewModel } from '../../viewmodels/useWelcomeViewModel';
import { colors } from '../../constants/colors';
import { typography } from '../../constants/typography';
import { spacing } from '../../constants/spacing';

export default function WelcomeScreen({ navigation, route }) {
  const userName = route?.params?.userName ?? 'aventureiro';

  const { activeDot, totalDots } = useWelcomeViewModel(() => {
    navigation.replace('Onboarding');
  });

  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.body}>
        <Image
          source={require('../../assets/logo.png')}
          style={styles.logo}
          resizeMode="contain"
        />

        <Text style={styles.title}>
          Bem-vinda,{' '}
          <Text style={styles.name}>{userName}!</Text>
        </Text>

        <Text style={styles.subtitle}>Pronta para sua próxima aventura?</Text>
      </View>

      <View style={styles.dots}>
        {Array.from({ length: totalDots }).map((_, i) => (
          <View key={i} style={[styles.dot, i <= activeDot && styles.dotActive]} />
        ))}
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.white,
    paddingBottom: spacing.xxxl,
  },

  body: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    paddingHorizontal: spacing.xl,
    gap: spacing.md,
  },

  logo: {
    width: 160,
    height: 160,
    marginBottom: spacing.xl,
  },

  title: {
    fontSize: typography.size.xl,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
    textAlign: 'center',
  },

  name: {
    color: colors.peachBurnt,
  },

  subtitle: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.regular,
    color: colors.grayText,
    textAlign: 'center',
  },

  dots: {
    flexDirection: 'row',
    justifyContent: 'center',
    gap: spacing.sm,
  },

  dot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    backgroundColor: colors.grayLight,
  },

  dotActive: {
    backgroundColor: colors.purplePrimary,
  },
});
