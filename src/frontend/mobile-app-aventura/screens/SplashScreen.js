import React, { useEffect, useRef } from 'react';
import { Animated, Image, Text, StyleSheet, StatusBar, Platform } from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing } from '../constants/spacing';
import { getAuthToken } from '../services/tokenStorageService';

const DISPLAY_DURATION = 1500;
const FADE_DURATION = 500;

export default function SplashScreen({ navigation }) {
  const opacity = useRef(new Animated.Value(1)).current;

  useEffect(() => {
    const timer = setTimeout(() => {
      Animated.timing(opacity, {
        toValue: 0,
        duration: FADE_DURATION,
        useNativeDriver: true,
      }).start(async () => {
        const token = await getAuthToken();
        navigation.replace(token ? 'MainTabs' : 'Auth');
      });
    }, DISPLAY_DURATION);

    return () => clearTimeout(timer);
  }, [navigation, opacity]);

  return (
    <Animated.View style={[styles.container, { opacity }]}>
      <StatusBar barStyle="light-content" backgroundColor={colors.purplePrimary} />

      <Image
        source={require('../assets/logo-branca-aventura-completa.png')}
        style={styles.logo}
        resizeMode="contain"
      />

      <Text style={styles.tagline}>Cultura · Teatro · Eventos</Text>
    </Animated.View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.purplePrimary,
    alignItems: 'center',
    justifyContent: 'center',
  },

  logo: {
    width: 180,
    height: 180,
  },

  tagline: {
    position: 'absolute',
    bottom: spacing.xxxl,
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.medium,
    color: colors.white,
    letterSpacing: 1,
  },
});
