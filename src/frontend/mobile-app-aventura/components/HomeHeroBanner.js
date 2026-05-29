import React, { useEffect, useRef, useState } from 'react';
import {
  Image,
  TouchableOpacity,
  View,
  StyleSheet,
  ScrollView,
  Dimensions,
} from 'react-native';

import { colors } from '../constants/colors';
import { spacing } from '../constants/spacing';

const { width } = Dimensions.get('window');

export default function HomeHeroBanner({
  banners = [],
  autoPlay = true,
  interval = 3000,
  onBannerPress,
}) {
  const scrollRef = useRef(null);
  const [activeIndex, setActiveIndex] = useState(0);

  const bannerWidth = width;

  useEffect(() => {
    if (!autoPlay || banners.length <= 1) return;

    const timer = setInterval(() => {
      const nextIndex = activeIndex === banners.length - 1 ? 0 : activeIndex + 1;

      scrollRef.current?.scrollTo({
        x: nextIndex * bannerWidth,
        animated: true,
      });

      setActiveIndex(nextIndex);
    }, interval);

    return () => clearInterval(timer);
  }, [activeIndex, autoPlay, banners.length, bannerWidth, interval]);

  function handleScroll(event) {
    const offsetX = event.nativeEvent.contentOffset.x;
    const index = Math.round(offsetX / bannerWidth);
    setActiveIndex(index);
  }

  if (!banners.length) {
    return <View style={styles.emptyBanner} />;
  }

  return (
    <View style={styles.container}>
      <ScrollView
        ref={scrollRef}
        horizontal
        pagingEnabled
        showsHorizontalScrollIndicator={false}
        onMomentumScrollEnd={handleScroll}
        scrollEventThrottle={16}
      >
        {banners.map((banner) => (
          <TouchableOpacity
            key={banner.id}
            activeOpacity={banner.showId ? 0.9 : 1}
            onPress={() => onBannerPress?.(banner)}
            style={{ width: bannerWidth, height: '100%' }}
          >
            <Image
              source={banner.image}
              style={[styles.image, { width: bannerWidth }]}
              resizeMode="cover"
            />
          </TouchableOpacity>
        ))}
      </ScrollView>

      <View style={styles.indicators}>
        {banners.map((banner, index) => (
          <View
            key={banner.id}
            style={[
              styles.indicator,
              index === activeIndex && styles.activeIndicator,
            ]}
          />
        ))}
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    width: '100%',
    height: 280,
    backgroundColor: colors.purpleSoft,
    position: 'relative',
    overflow: 'hidden',
  },

  image: {
    height: '100%',
  },

  emptyBanner: {
    width: '100%',
    height: 210,
    backgroundColor: colors.purpleSoft,
  },

  indicators: {
    position: 'absolute',
    right: spacing.lg,
    bottom: spacing.md,
    flexDirection: 'row',
    gap: spacing.xs,
  },

  indicator: {
    width: 7,
    height: 7,
    borderRadius: 999,
    backgroundColor: colors.white,
    opacity: 0.6,
  },

  activeIndicator: {
    backgroundColor: colors.peachBurnt,
    opacity: 1,
  },
});