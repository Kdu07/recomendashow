import React, { useRef, useEffect } from 'react';
import { View, Text, Image, StyleSheet, SafeAreaView, Animated } from 'react-native';

import { useWelcomeViewModel } from '../../viewmodels/useWelcomeViewModel';
import { colors } from '../../constants/colors';
import { typography } from '../../constants/typography';
import { spacing } from '../../constants/spacing';

const DOT_COLORS = [colors.purpleLight, colors.purpleSecondary, colors.purplePrimary];
const DOT_DELAYS = [0, 1666, 3333];

export default function WelcomeScreen({ navigation, route }) {
  const userName = route?.params?.userName ?? 'aventureiro';

  useWelcomeViewModel(() => {
    navigation.replace('MainTabs');
  });

  const dotAnims = useRef(DOT_DELAYS.map(() => new Animated.Value(0))).current;

  useEffect(() => {
    const timers = DOT_DELAYS.map((delay, i) =>
      setTimeout(() => {
        Animated.spring(dotAnims[i], {
          toValue: 1,
          friction: 5,
          tension: 80,
          useNativeDriver: true,
        }).start();
      }, delay)
    );

    return () => timers.forEach(clearTimeout);
  }, []);

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
        {dotAnims.map((anim, i) => {
          const scale = anim.interpolate({
            inputRange: [0, 1],
            outputRange: [0.3, 1],
          });

          return (
            <Animated.View
              key={i}
              style={[
                styles.dot,
                {
                  backgroundColor: DOT_COLORS[i],
                  opacity: anim,
                  transform: [{ scale }],
                },
              ]}
            />
          );
        })}
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
    gap: spacing.md,
  },

  dot: {
    width: 10,
    height: 10,
    borderRadius: 5,
  },
});
